using System.Collections;
using System.Collections.Generic;
using Generic;
using PassiveSkill;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : Unit
{
    public override void SetUp(int index, string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(index, newName, unitStat, newWeapon, unitModel, passiveList);
        
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnChanged);
        onSelectedChanged.AddListener(() => UIManager.instance.onActionChanged.Invoke());
        OnAimStart.AddListener((a, p) => UIManager.instance.onActionChanged.Invoke());
        OnAimEnd.AddListener((a) => UIManager.instance.onActionChanged.Invoke());
        PlayerEvents.OnStartedQuest.AddListener((quest) => OnForceFinish());
        PlayerEvents.OnSuccessQuest.AddListener((quest) => OnForceFinish());
        PlayerEvents.OnFailedQuest.AddListener((quest) => OnForceFinish());
        FieldSystem.onCombatFinish.AddListener((isWin) => OnForceFinish());

        TileEffectManager.instance.SetPlayer(this);

        FieldSystem.onStageAwake.AddListener(ReloadSight);
        onWeaponChange.AddListener((w)=> { ReloadSight(); });
        stat.OnChangedStat.AddListener((type) => { PlayerEvents.OnChangedStat?.Invoke(stat, type); });
    }
    public void Update()
    {
        if (GameManager.instance.CompareState(GameState.EDITOR)) return;
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (UIManager.instance.isMouseOverUI) return;
        // if (HasStatusEffect(StatusEffectType.Stun)) EndTurn();
        bool wasAimmed = isAimming;

        bool isMouseOnTile = TryGetMouseOverTilePos(out Vector3Int onMouseTilePos);

        if (isMouseOnTile && GetSelectedAction().GetActionType() is
                ActionType.Attack or ActionType.Fanning or ActionType.ItemUsing)
        {
            Unit target = FieldSystem.unitSystem.GetUnit(onMouseTilePos);
            if (target is not Player and not null)
            {
                transform.LookAt(Hex.Hex2World(onMouseTilePos), Vector3.up);
                isAimming = true;
                
            }
            else
            {
                isAimming = false;
            }
        }
        if (wasAimmed != isAimming) 
        {
            if (isAimming) OnAimStart.Invoke(GetSelectedAction(), onMouseTilePos);
            else OnAimEnd.Invoke(GetSelectedAction());
        }

        if (Input.GetMouseButtonDown(0) && isMouseOnTile)
        {
            var actionSuccess = TryExecuteUnitAction(onMouseTilePos);
            
            if(actionSuccess is false) ClearBusy();
            else SetBusy();
        }

        if (Input.GetMouseButton(1) && GameManager.instance.CompareState(GameState.COMBAT))
        {
            SelectAction(GetAction<IdleAction>());
            UIManager.instance.combatUI.ClosePopupWindow();
        }
    }

    protected override void DeadCall(Unit unit)
    {
        stat.OnChangedStat.RemoveAllListeners();
        PlayerEvents.OnStartedQuest.RemoveListener((quest) => OnForceFinish());
        PlayerEvents.OnSuccessQuest.AddListener((quest) => OnForceFinish());
        PlayerEvents.OnFailedQuest.AddListener((quest) => OnForceFinish());
        base.DeadCall(unit);
    }

    public override void StartTurn()
    {
        base.StartTurn();
        
        if (GameManager.instance.CompareState(GameState.WORLD))
        {
            SelectAction(GetAction<MoveAction>());
            ReloadSight();
            PlayerEvents.OnProcessedWorldTurn.Invoke(GameManager.instance.runtimeWorldData.worldTurn);
        }
    }

    public void ContinueWorldTurn()
    {
        stat.SetOriginalStat(StatType.CurActionPoint, GameManager.instance.user.Stat.curActionPoint);
        //onStatChanged.Invoke();
        UIManager.instance.onPlayerStatChanged.Invoke();
        PlayerEvents.OnChangedStat?.Invoke(stat, StatType.CurActionPoint);
        stat.OnChangedStat.Invoke(StatType.CurActionPoint);
        
        SelectAction(GetAction<MoveAction>());
    }
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static bool TryGetMouseOverTilePos(out Vector3Int pos)
    {
        if (UIManager.instance.isMouseOverUI)
        {
            pos = Vector3Int.zero;
            return false;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (H9Math.IsRayIntersectingPlane(ray, out float t, 0))
        {
            Vector3 collisionPosition = ray.origin + ray.direction * t;
            Vector3Int collisionHex = Hex.Round(Hex.World2Hex(collisionPosition));

            Tile tile = FieldSystem.tileSystem.GetTile(collisionHex);
            if (tile is null)
            {
                pos = Vector3Int.zero;
                return false;
            }

            if (GameManager.instance.CompareState(GameState.WORLD) || tile.inSight)
            {
                pos = tile.hexPosition;
                return true;
            }
        }

        pos = Vector3Int.zero;
        return false;
    }

    private int _cacheSightRange = -1;
    public void ReloadSight()
    {

        int availableMaxSightRange =
            Mathf.Max(_cacheSightRange, stat.sightRange);
        _cacheSightRange = stat.sightRange;
        
        //한칸 움직일때마다 호출되므로, 보였다가 시야에서 사라지는 경우는 sightRange + 1로 탐색 범위에 포함 시킬 수 있음
        IEnumerable<Tile> allTile =
            FieldSystem.tileSystem.GetTilesInRange(hexPosition, availableMaxSightRange + 1);

        foreach (var tile in allTile)
        {
            var isInDistance = Hex.Distance(hexTransform.position, tile.hexPosition) <= stat.sightRange;
            var isInMoreClosedDistance = Hex.Distance(hexTransform.position, tile.hexPosition) <= stat.sightRange - 1;

            if (isInMoreClosedDistance) // 시야 끝거리에서만 보이지말고 한칸 들어오게 해달라는 미친 PM의 요청
            {
                PlayerEvents.OnEnteredTileinSight?.Invoke(tile);
                foreach (TileObject tileObj in tile.tileObjects)
                    if (tileObj is Link l) PlayerEvents.OnEnteredLinkinSight?.Invoke(l); // 으아악 미친코드다
            }

            if(GameManager.instance.CompareState(GameState.WORLD) && tile.inSight) continue;
            bool isInVision = FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition);
            tile.inSight = isInVision && isInDistance;
        }
        
        foreach (var unit in FieldSystem.unitSystem.units)
        {
            CalculateUnitVision(unit);
        }
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);

        UIManager.instance.onPlayerStatChanged.Invoke();
        GameManager.instance.user.Stat = stat;
    }

    /// <summary>
    /// 플레이어가 target Unit을 볼 수 있는지 체크합니다. unit의 meshVisible은 오직 이 함수를 통해서만 수정되어야 합니다.
    /// </summary>
    public void CalculateUnitVision(Unit target)
    {
        if (target is Player) return;
        
        // 1. 보스몬스터의 vanish trigger 고려
        if (target.GetVanishTrigger())
        {
            target.SetMeshVisible(this, false);
            return;
        }
        
        // 2. 부쉬와 관련된 요소로 target을 볼 수 있는가?
        if (CanSeeOverBush() is false)
        {
            target.SetMeshVisible(this, false);
            return;
        }
        
        
        // 3. 플레이어의 vision Range 처리 고려
        if(Hex.Distance(hexPosition, target.hexPosition) > stat.sightRange)
        {
            target.SetMeshVisible(this, false);
            return;
        }
        
        // 4. 플레이어의 visionCheck 처리 고려
        // 비용이 가장 크기 때문에 마지막으로 미룸
        bool visionCheck =
            FieldSystem.tileSystem.VisionCheck
                (hexPosition, target.hexPosition);
        
        target.SetMeshVisible(this, visionCheck);
        return;

        // 부쉬와 관련된 요소로 target을 볼 수 있는가?
        bool CanSeeOverBush()
        {
            // 만약 쟤가 부쉬에 없다면 True
            if (BushSystem.Instance.IsBush(target.hexPosition) is false)
            {
                return true;
            }
            
            // 둘이 같은 Bush에 있다면 True
            if (BushSystem.Instance.IsSameGroup(this, target))
            {
                return true;
            }
            
            // 둘다 다른 부쉬에 있다는건데, Attak 같은 행동을 해서 보일 수 있는가?
            if (BushSystem.Instance.IsNonHideUnit(target))
            {
                return true;
            }
            
            // 어 못봐
            return false;
        }
    }

    #region UNITY_EVENT
    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            CalculateUnitVision(unit);
        }
    }

    protected override void OnMoved(Vector3Int from, Vector3Int to, Unit unit)
    {
        base.OnMoved(from, to, unit);
        
        ReloadSight();
        PlayerEvents.OnMovedPlayer?.Invoke(to);
    }

    private void OnTurnChanged()
    {
        if(IsMyTurn() is false) SelectAction(GetAction<IdleAction>());
    }
    
    private void OnStatusEffectChanged()
    {
        ReloadSight();
    }

    private void OnForceFinish()
    {
        this.activeUnitAction?.ForceFinish();
    }
    #endregion
}
