using KieranCoppins.DecisionTrees;

public class OutOfAmmo : H9Function<bool>
{
    public override bool Invoke()
    {
        return ai.GetUnit().weapon.currentAmmo <= 0;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if unit is out of ammo";
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Check if unit is out of ammo";
    }
}