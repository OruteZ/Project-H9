using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class ExecuteAction : H9Action
{
    [HideInInspector] [SerializeField] private Function<IUnitAction> action;

    public ExecuteAction(Function<IUnitAction> action)
    {
        this.action = action;
    }

    public override IEnumerator Execute()
    {
        
        yield break;
    }
    
    public IUnitAction GetAction()
    {
        return action.Invoke();
    }
}