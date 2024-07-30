using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetMoveCount : H9Function<float>
{
    public override float Invoke()
    {
        return ai.MoveCount;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current attack count";
    }
}