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
        onMoved.AddListener((p) =>  PlayerEvents.OnMovedPlayer?.Invoke(p.hexPosition));
        onStatusEffectChanged.AddListener(OnStatusEffectChanged);
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnChanged);
        onSelectedChanged.AddListener(() => UIManager.instance.onActionChanged.Invoke());
        PlayerEvents.OnStartedQuest.AddListener((quest) => OnForceFinish());
        PlayerEvents.OnSuccessQuest.AddListener((quest) => OnForceFinish());
        PlayerEvents.OnFailedQuest.AddListener((quest) => OnForceFinish());

        TileEffectManager.instance.SetPlayer(this);

        FieldSystem.onStageAwake.AddListener(ReloadSight);
        stat.OnChangedStat.AddListener((type) => { PlayerEvents.OnChangedStat?.Invoke(stat, type); });
    }
    public void Update()
    {
        if (GameManager.instance.CompareState(GameState.Editor)) return;
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (UIManager.instance.isMouseOverUI) return;
        // if (HasStatusEffect(StatusEffectType.Stun)) EndTurn();

        var isMouseOnTile = TryGetMouseOverTilePos(out var onMouseTilePos);

        if (isMouseOnTile && GetSelectedAction().GetActionType() is
                ActionType.Attack or ActionType.Fanning or ActionType.ItemUsing)
        {
            var target = FieldSystem.unitSystem.GetUnit(onMouseTilePos);
            if (target is not Player and not null)
            {
                transform.LookAt(Hex.Hex2World(onMouseTilePos), Vector3.up);
            }
        }

        if (Input.GetMouseButtonDown(0) && isMouseOnTile) 
        {
            var actionSuccess = TryExecuteUnitAction(onMouseTilePos);
            
            if(actionSuccess is false) ClearBusy();
            else SetBusy();
        }

        if (Input.GetMouseButton(1) && GameManager.instance.CompareState(GameState.Combat))
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
        
        if (GameManager.instance.CompareState(GameState.World))
        {
            SelectAction(GetAction<MoveAction>());
            ReloadSight();
            PlayerEvents.OnProcessedWorldTurn.Invoke(GameManager.instance.runtimeWorldData.worldTurn);
        }
    }

    public void ContinueWorldTurn()
    {
        stat.SetOriginalStat(StatType.CurActionPoint, GameManager.instance.worldAp);
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
        if (Physics.Raycast(ray, out var hit, float.MaxValue ,layerMask : LayerMask.GetMask("Tile")))
        {
            var tile = hit.collider.GetComponent<Tile>();

            if (tile is null)
            {
                pos = Vector3Int.zero;
                return false;
            }

            if (GameManager.instance.CompareState(GameState.World))
            {
                pos = tile.hexPosition;
                return true;
            }

            if (tile.inSight)
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
                foreach (var tileObj in tile.tileObjects)
                    if (tileObj is Link) PlayerEvents.OnEnteredLinkinSight?.Invoke(tileObj as Link); // 으아악 미친코드다
            }

            if(GameManager.instance.CompareState(GameState.World) && tile.inSight) continue;
            var isInVision = FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition);
            tile.inSight = isInVision && isInDistance;

#if UNITY_EDITOR
            var unitVision = FieldSystem.unitSystem.GetUnit(tile.hexPosition);
            if (unitVision != null)
            {
                Debug.Log("Unit : " + unitVision.gameObject.name);
                Debug.Log("Vision Check = " + FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition));
                Debug.Log("Distance = " + Hex.Distance(hexTransform.position, tile.hexPosition));
            }
#endif
        }
    }

    public override void TakeDamage(int damage, Unit attacker, eDamageType.Type type = eDamageType.Type.Default)
    {
        base.TakeDamage(damage, attacker, type);

        UIManager.instance.onPlayerStatChanged.Invoke();
        GameManager.instance.playerStat = stat;
    }

    #region UNITY_EVENT
    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.isVisible = FieldSystem.tileSystem.VisionCheck(hexPosition, unit.hexPosition) &&
                             Hex.Distance(hexTransform.position, unit.hexPosition) <= stat.sightRange;
        }
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
        foreach (var obj in FieldSystem.tileSystem.GetTile(hexPosition).tileObjects) 
        { 
            obj.OnCollision(unit);
        }
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
