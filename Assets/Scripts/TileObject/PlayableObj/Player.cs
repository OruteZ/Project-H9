using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Unit
{
    private bool _isBusy;
    public bool IsBusy
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

    [Header("Status")]
    public int actionPoint;

    public override void SetUp(string newName)
    {
        base.SetUp(newName);
        
        UnitSystem.
            onAnyUnitMoved.
            AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);
    }
    public override void Updated()
    {
        if (IsBusy) return;
        if (!CombatManager.Instance.IsPlayerTurn()) return;
        
        // if (Input.GetKeyDown(KeyCode.A)) SelectAction(GetAction<MoveAction>());
        // if (Input.GetKeyDown(KeyCode.D)) SelectAction(GetAction<AttackAction>());

        if (Input.GetMouseButtonDown(0) && !UIManager.Instance.isMouseOverUI)
        {
            target = GetMouseOverTile();
            if (target == null)
            {
#if true
                Debug.Log("Target tile is null");
#endif
                return;
            }

            if (activeUnitAction.CanExecute(target.Position) && actionPoint >= activeUnitAction.GetCost())
            {
                SetBusy();
                activeUnitAction.Execute(target.Position, FinishAction);
            }
        }   
    }

    public override void StartTurn()
    {
#if UNITY_EDITOR
        Debug.Log("Player Turn Started");
#endif 
        actionPoint = 10; //todo : 공식 가져와서 행동력 계산하기

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
        IsBusy = true;
    }

    private void ClearBusy()
    {
        IsBusy = false;
    }
    private void FinishAction()
    {
        ClearBusy();
        actionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(actionPoint);
    }

    public Tile GetMouseOverTile()
    {
        RaycastHit hit; 
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }

    private void ReloadSight()
    {
        var allTile = TileSystem.GetAllTiles();

        foreach (var tile in allTile)
        {
            tile.InSight = 
                TileSystem.VisionCast(hexTransform.position, tile.Position) &&
                Hex.Distance(hexTransform.position, tile.Position) <= sightRange;
        }
    }
    
    public override void OnHit(int damage)
    {
    }

    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit != this)
        {
            unit.IsVisible = TileSystem.VisionCast(Position, unit.Position) &&
                             Hex.Distance(hexTransform.position, unit.Position) <= sightRange;
        }
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
    }
}
