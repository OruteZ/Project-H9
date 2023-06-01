using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Unit
{
    private bool _isBusy;
    public bool isBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                onBusyChanged.Invoke();
            }
        }
    }
    
    [HideInInspector] public UnityEvent onSelectedChanged;

    [HideInInspector] public Tile target;

    public override void SetUp(string newName, UnitStat unitStat, int weaponIndex)
    {
        base.SetUp(newName, unitStat, weaponIndex);
        
        unitSystem.
            onAnyUnitMoved.
            AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
    }
    public override void Updated()
    {
        if (isBusy) return;
        if (!IsMyTurn()) return;
        
        // if (Input.GetKeyDown(KeyCode.A)) SelectAction(GetAction<MoveAction>());
        // if (Input.GetKeyDown(KeyCode.D)) SelectAction(GetAction<AttackAction>());

        
        if (Input.GetMouseButtonDown(0)) //todo : UIManager.Instance.IsMouseOverUI;
        {
            target = GetMouseOverTile();
            if (target == null)
            {
#if true
                Debug.Log("Target tile is null");
#endif
                return;
            }

            if (activeUnitAction.CanExecute(target.position) && currentActionPoint >= activeUnitAction.GetCost())
            {
                SetBusy();
                activeUnitAction.Execute(target.position, FinishAction);
            }
        }   
    }

    public override void StartTurn()
    {
#if UNITY_EDITOR
        Debug.Log("Player Turn Started");
#endif
        currentActionPoint = stat.concentration;

        activeUnitAction = null;
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

    public IUnitAction GetSelectedAction()
    {
        return activeUnitAction;
    }

    private void SetBusy()
    {
        isBusy = true;
    }

    private void ClearBusy()
    {
        isBusy = false;
    }
    private void FinishAction()
    {
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
    }

    public Tile GetMouseOverTile()
    {
        RaycastHit hit; 
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f ,layerMask : LayerMask.GetMask("Tile")))
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile.inSight) return tile;
        }

        return null;
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
        if(unit != this)
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
