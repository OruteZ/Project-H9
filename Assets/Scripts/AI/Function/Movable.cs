using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class Movable : H9Function<bool>
{
    [SerializeField, HideInInspector] private Function<Vector3Int> targetPos;
    
    public Movable(Function<Vector3Int> targetPos)
    {
        this.targetPos = targetPos;
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        targetPos.Initialise(metaData);
    }
    
    public override bool Invoke()
    {
        MoveAction action = ai.GetUnit().GetAction<MoveAction>();
        if(action is null) return false;
        
        return action.CanExecute(targetPos.Invoke()) && action.IsSelectable();
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if unit has action points";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Check if unit can move to target position"; 
    }
}
