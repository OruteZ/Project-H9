using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Unit
{
    private EnemyAI _ai;

    protected override void Awake()
    {
        base.Awake();
        
        _ai = GetComponent<EnemyAI>();
    }

    public override void SetUp(string newName, UnitStat unitStat, Weapon weapon)
    {
        base.SetUp(newName, unitStat, weapon);
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;

        activeUnitAction = _ai.SelectAction(out var target);
        if (activeUnitAction is IdleAction) FinishAction();
        
        if (TryExecuteUnitAction(target, FinishAction))
        {
            SetBusy();
        }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");

        StartCoroutine(UITestEndTurn());
        return;//for ui test

        #endif
        currentActionPoint = stat.actionPoint;

        activeUnitAction = null;
    }

    public override void GetDamage(int damage)
    {
        Debug.Log(damage + " 데미지 받음");
    }
    
    private void FinishAction()
    {
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
        
        if (currentActionPoint == 0)
        {
            FieldSystem.turnSystem.EndTurn();
        }
    }

    IEnumerator UITestEndTurn() 
    {
        while(true) 
        {
            yield return new WaitForSeconds(2.0f * Time.timeScale);
            FieldSystem.turnSystem.EndTurn();
            yield break;
        }
    }
}
