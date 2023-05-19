using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Outline _outline;

    private Outline OutLine
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
        CombatSystem.
            Instance.
            Player.
            onSelectedChanged.
            AddListener(CheckActiveAction);
    }

    public void SetAction(IUnitAction unitAction)
    {
        this.unitAction = unitAction;
        actionName = unitAction.GetActionType().ToString();
        CheckActiveAction();
    }

    public IUnitAction GetAction()
    {
        return unitAction;
    }

    private void CheckActiveAction()
    {
        #if UNITY_EDITOR
        Debug.Log(gameObject.name + " " + "Check Active");
        Debug.Log(unitAction.GetActionType());
        Debug.Log(CombatSystem.
            Instance.
            Player.GetSelectedAction().GetActionType());
        #endif
        OutLine.enabled = 
            unitAction.GetActionType() 
            == 
            CombatSystem.
            Instance.
            Player.GetSelectedAction().GetActionType();
    }

    public void Select()
    {
        CombatSystem.
            Instance.
            Player.SelectAction(unitAction);
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
