using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : Unit
{
    [HideInInspector] public UnityEvent onSelectedChanged;
    public override void SetUp(string newName, UnitStat unitStat, Weapon newWeapon)
    {
        base.SetUp(newName, unitStat, newWeapon);
        
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
        TileEffectManager.instance.SetPlayer(this);
    }
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (UIManager.instance.isMouseOverUI) return;
        
        if (Input.GetMouseButtonDown(0) && TryGetMouseOverTilePos(out var targetPos)) 
        {
            var actionSuccess = TryExecuteUnitAction(targetPos, FinishAction);
            if (actionSuccess) SetBusy();
        }   
    }

    public override void StartTurn()
    {
#if UNITY_EDITOR
        Debug.Log("Player Turn Started");
#endif
        
        hasAttacked = false;
        currentActionPoint = stat.actionPoint;
        ReloadSight();
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            SelectAction(GetAction<IdleAction>());
        }
        else
        {
            SelectAction(GetAction<MoveAction>());
        }
    }
    
    public void SelectAction(IUnitAction action)
    {
        //if (activeUnitAction == action) return;
        if (action.IsSelectable() is false) return;
        if (action.GetCost() > currentActionPoint)
        {  
            Debug.Log("Cost is loss, Cost is " + action.GetCost());
            return;
        }
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif

        activeUnitAction = action;
        onSelectedChanged.Invoke();

        if (activeUnitAction.CanExecuteImmediately())
        {
            TryExecuteUnitAction(Vector3Int.zero, FinishAction);
        }
        UIManager.instance.combatUI.SetCombatUI();
    }

    private void FinishAction()
    {
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
        
        ClearBusy();
        if(GameManager.instance.CompareState(GameState.Combat))
        {
            var idleAction = GetAction<IdleAction>();
            SelectAction(idleAction is null ? GetAction<MoveAction>() : idleAction);
        }
        else
        {
            SelectAction(GetAction<MoveAction>());
        }
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

    private void ReloadSight()
    {
        //한칸 움직일때마다 호출되므로, 보였다가 시야에서 사라지는 경우는 sightRange + 1로 탐색 범위에 포함 시킬 수 있음
        IEnumerable<Tile> allTile =
            FieldSystem.tileSystem.GetTilesInRange(hexPosition, stat.sightRange + 1);

        foreach (var tile in allTile)
        {
            tile.inSight = 
                FieldSystem.tileSystem.VisionCheck(hexTransform.position, tile.hexPosition) &&
                Hex.Distance(hexTransform.position, tile.hexPosition) <= stat.sightRange;
        }
    }
    
    public override void GetDamage(int damage)
    {
    }

    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.isVisible = FieldSystem.tileSystem.VisionCheck(hexPosition, unit.hexPosition) &&
                             Hex.Distance(hexTransform.position, unit.hexPosition) <= stat.sightRange;
        }
        
        Debug.Log("On Any Unit Moved : Invoke");
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
        foreach (var obj in FieldSystem.tileSystem.GetTile(hexPosition).interactiveObjects) 
        { 
            obj.OnCollision(unit);
        }
    }
}
