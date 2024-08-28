using System;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetTurnNumber : H9Function<float>
{

    public override float Invoke()
    {
        return FieldSystem.turnSystem.turnNumber;
    }
    
    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current turn number";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Get current turn number";
    }
}