using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetMaxMoveCount : H9Function<int>
{
    public override int Invoke()
    {
        return ai.MoveCount;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current attack count";
    }
}