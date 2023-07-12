using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Outline _outline;
    private IUnitAction _unitAction;
    public string actionName;
    
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

    private void Start()
    {
        FieldSystem.unitSystem.GetPlayer().onSelectedChanged.
            AddListener(CheckActiveAction);
    }

    public void SetAction(IUnitAction newAction)
    {
        _unitAction = newAction;
        actionName = newAction.ToString();
        CheckActiveAction();
    }

    public IUnitAction GetAction()
    {
        return _unitAction;
    }

    private void CheckActiveAction()
    {
        outLine.enabled = 
            _unitAction.GetActionType() == FieldSystem.unitSystem.GetPlayer().GetSelectedAction().GetActionType();
    }

    public void Select()
    {
        FieldSystem.unitSystem.GetPlayer().SelectAction(_unitAction);
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
