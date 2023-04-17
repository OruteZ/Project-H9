using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Unit
{
    private bool isBusy;
    private IUnitAction selectedAction;

    public UnityEvent onSelectedChanged;
    public UnityEvent onBusyChanged;

    [HideInInspector] public Tile target;

    [Header("Status")]
    public int actionPoint;
    
    
    public override void SetUp(string newName, CombatSystem system)
    {
        base.SetUp(newName, system);
    }
    public override void Updated()
    {
        if (isBusy) return;
        if (system.turnOwner != this) return;

        if (Input.GetMouseButton(0)) // todo : UI에 마우스가 올라가있지 않는가 조건 추가
        {
            var targetTile = GetMouseOverTile();
            if (targetTile == null) return;

            if (selectedAction.CanExecute(targetTile.position) && actionPoint >= selectedAction.GetCost())
            {
                SetBusy();
                selectedAction.Execute(target.position, ClearBusy);
            }
        }   
    }

    public override void StartTurn()
    {
        actionPoint = 100; //todo : 공식 가져와서 행동력 계산하기
        SelectAction(GetAction<MoveAction>()); 
    }

    private void SelectAction(IUnitAction action)
    {
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
        isBusy = true;
        onBusyChanged.Invoke();
    }

    private void ClearBusy()
    {
        isBusy = false;
        onBusyChanged.Invoke();
    }

    public bool IsBusy()
    {
        return isBusy;
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
