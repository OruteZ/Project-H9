using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetAmmo : H9Function<float>
{
    public override float Invoke()
    {
        return ai.GetUnit().weapon.CurrentAmmo;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current ammo count";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Get current ammo count";
    }
}