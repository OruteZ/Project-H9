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

    public override void SetUp(string newName)
    {
        base.SetUp(newName);
        
        unitSystem.
            onAnyUnitMoved.
            AddListener(OnAnyUnitMoved);
        onMoved.AddListener(OnMoved);

        weapon = Weapon.Clone(WeaponDataBase.Instance.weaponList[0], unit : this);
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
        currentActionPoint = concentration;

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
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
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
        var allTile = tileSystem.GetAllTiles();

        foreach (var tile in allTile)
        {
            tile.inSight = 
                tileSystem.VisionCast(hexTransform.position, tile.position) &&
                Hex.Distance(hexTransform.position, tile.position) <= sightRange;
        }
    }
    
    public override void OnHit(int damage)
    {
    }

    private void OnAnyUnitMoved(Unit unit)
    {
        if(unit != this)
        {
            unit.isVisible = tileSystem.VisionCast(position, unit.position) &&
                             Hex.Distance(hexTransform.position, unit.position) <= sightRange;
        }
    }

    private void OnMoved(Unit unit)
    {
        ReloadSight();
    }
}
