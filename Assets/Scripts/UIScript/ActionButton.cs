using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Outline _outline;
    private Player _player => CombatSystem.instance.unitSystem.GetPlayer();
    public Outline outLine
    {
        get
        {
            if(_outline == null)
            {
                _outline = GetComponent<Outline>();
            }

            return _outline;
        }
    }
    
    public IUnitAction unitAction;
    

    public string actionName;

    private void Start()
    {
        _player.onSelectedChanged.
            AddListener(CheckActiveAction);
    }

    public void SetAction(IUnitAction newAction)
    {
        this.unitAction = newAction;
        actionName = newAction.ToString();
        CheckActiveAction();
    }

    public IUnitAction GetAction()
    {
        return unitAction;
    }

    private void CheckActiveAction()
    {
        outLine.enabled = 
            unitAction.GetActionType() == _player.GetSelectedAction().GetActionType();
    }

    public void Select()
    {
        _player.SelectAction(unitAction);
    }

    public void OnMouseEnter()
    {
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }
}
