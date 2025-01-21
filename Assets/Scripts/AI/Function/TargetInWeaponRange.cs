using KieranCoppins.DecisionTrees;
using UnityEngine;

public class TargetInWeaponRange : H9Function<bool>
{
    [SerializeField, HideInInspector] private Function<Vector3Int> target;
    
    public TargetInWeaponRange(Function<Vector3Int> target)
    {
        this.target = target;
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        target.Initialise(metaData);
    }
    
    public override DecisionTreeEditorNodeBase Clone()
    {
        var c = Instantiate(this);
        c.target = (Function<Vector3Int>) target.Clone();
        
        return c;
    }
    
    
    public override bool Invoke()
    {
        return ai.GetUnit().weapon.GetRange() >= 
               Hex.Distance(ai.GetUnit().hexPosition, target.Invoke());
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if target is in weapon range";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Check if target is in weapon range";
    }
}