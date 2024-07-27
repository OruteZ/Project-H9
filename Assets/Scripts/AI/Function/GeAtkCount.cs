using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetMaxAtkCount : H9Function<int>
{
    public override int Invoke()
    {
        return ai.AtkCount;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current attack count";
    }
}