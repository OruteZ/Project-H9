using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class IsExecutable : H9Function<bool>
{
    [SerializeField, HideInInspector] private Function<IUnitAction> action;
    [SerializeField, HideInInspector] private Function<Vector3Int> targetPos;
    
    public IsExecutable(Function<IUnitAction> action, Function<Vector3Int> targetPos)
    {
        this.action = action;
        this.targetPos = targetPos;
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        action.Initialise(metaData);
        targetPos.Initialise(metaData);
    }
    
    public override bool Invoke()
    {
        return action.Invoke().IsSelectable() && 
               action.Invoke().CanExecute(targetPos.Invoke());
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if unit has action points";
    }
}
