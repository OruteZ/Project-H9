using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Unit
{
    [HideInInspector] public UnityEvent onSelectedChanged;
    public override void SetUp(string newName, UnitStat unitStat, int weaponIndex)
    {
        base.SetUp(newName, unitStat, weaponIndex);
        
        unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
    }
    public override void Updated()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        
        //todo : UIManager.Instance.IsMouseOverUI : return;
        
        // if (Input.GetKeyDown(KeyCode.A)) SelectAction(GetAction<MoveAction>());
        // if (Input.GetKeyDown(KeyCode.D)) SelectAction(GetAction<AttackAction>());

        
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
        currentActionPoint = stat.actionPoint;
        SelectAction(GetAction<MoveAction>());
        ReloadSight();
    }
    
    public void SelectAction(IUnitAction action)
    {
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif
        if (activeUnitAction == action) return;

        activeUnitAction = action;
        onSelectedChanged.Invoke();
    }

    private void FinishAction()
    {
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static bool TryGetMouseOverTilePos(out Vector3Int pos)
    {
        RaycastHit hit; 
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f ,layerMask : LayerMask.GetMask("Tile")))
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile.inSight)
            {
                pos = tile.position;
                return true;
            }
        }

        pos = Vector3Int.zero;
        return false;
    }

    private void ReloadSight()
    {
        var allTile = tileSystem.GetAllTiles();

        foreach (var tile in allTile)
        {
            tile.inSight = 
                tileSystem.VisionCast(hexTransform.position, tile.position) &&
                Hex.Distance(hexTransform.position, tile.position) <= stat.sightRange;
        }
    }
    
    public override void GetDamage(int damage)
    {
    }

    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit is not Player)
        {
            unit.isVisible = tileSystem.VisionCast(position, unit.position) &&
                             Hex.Distance(hexTransform.position, unit.position) <= stat.sightRange;
        }
        
        Debug.Log("On Any Unit Moved : Invoke");
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
        foreach (var obj in tileSystem.GetTile(position).objects) 
        { 
            obj.OnCollision(unit);
        }
    }
}
