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

    private IUnitAction selectedAction;

    public UnityEvent onSelectedChanged;
    public UnityEvent onBusyChanged;
    public UnityEvent onCostChanged;

    [HideInInspector] public Tile target;

    [Header("Status")]
    public int actionPoint;

    public override void SetUp(string newName, CombatSystem system)
    {
        base.SetUp(newName, system);
    }
    public override void Updated()
    {
        if (IsBusy) return;
        if (system.turnOwner != this) return;

        if (Input.GetMouseButtonDown(0) && !UIManager.Instance.IsMouseOverUI())
        {
            var targetTile = GetMouseOverTile();
            if (targetTile == null)
            {
#if true
                Debug.Log("Target tile is null");
#endif
                return;
            }

            if (selectedAction.CanExecute(targetTile.position) && actionPoint >= selectedAction.GetCost())
            {
                SetBusy();
                selectedAction.Execute(targetTile.position, FinishAction);
            }
        }   
    }

    public override void StartTurn()
    {
#if UNITY_EDITOR
        Debug.Log("Player Turn Started");
#endif 
        actionPoint = 1; //todo : 공식 가져와서 행동력 계산하기
        SelectAction(GetAction<MoveAction>()); 
    }

    private void SelectAction(IUnitAction action)
    {
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif
        
        if (selectedAction == action) return;

        selectedAction = action;
        onSelectedChanged.Invoke();
    }

    public IUnitAction GetSelectedAction()
    {
        return selectedAction;
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
        actionPoint -= selectedAction.GetCost();
        onCostChanged.Invoke();
        if (actionPoint <= 0)
        {
            system.EndTurn();
        }
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
}
