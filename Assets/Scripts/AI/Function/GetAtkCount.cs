using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetAtkCount : H9Function<float>
{
    public override float Invoke()
    {
        return ai.AtkCount;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current attack count";
    }
}