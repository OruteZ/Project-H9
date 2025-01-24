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
        
        onMoved.AddListener(OnMoved);
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

#if UNITY_EDITOR
            Unit unitVision = FieldSystem.unitSystem.GetUnit(tile.hexPosition);
            if (unitVision != null)
            {
                //Debug.Log("Unit : " + unitVision.gameObject.name);
                //Debug.Log("Vision Check = " + FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition));
                //Debug.Log("Distance = " + Hex.Distance(hexTransform.position, tile.hexPosition));
            }
#endif
        }
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);

        UIManager.instance.onPlayerStatChanged.Invoke();
        GameManager.instance.user.Stat = stat;
    }

    #region UNITY_EVENT
    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.meshVisible = FieldSystem.tileSystem.VisionCheck(hexPosition, unit.hexPosition, true) &&
                             Hex.Distance(hexTransform.position, unit.hexPosition) <= stat.sightRange;
        }
    }

    private void OnMoved(Vector3Int from, Vector3Int to, Unit unit)
    {
        ReloadSight();
        
        List<TileObject> fromObjs = FieldSystem.tileSystem.GetTile(from).tileObjects;
        for (int index = 0; index < fromObjs.Count; index++)
        {
            TileObject obj = fromObjs[index];
            obj.OnHexCollisionExit(unit);
            
            // if obj destroyed, remove from list
            if (obj == null)
            {
                fromObjs.RemoveAt(index);
                index--;
            }
        }
        
        List<TileObject> toObjs = FieldSystem.tileSystem.GetTile(to).tileObjects;
        for (int index = 0; index < toObjs.Count; index++)
        {
            TileObject obj = toObjs[index];
            obj.OnHexCollisionEnter(unit);
            
            // if obj destroyed, remove from list
            if (obj == null)
            {
                toObjs.RemoveAt(index);
                index--;
            }
        }

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
