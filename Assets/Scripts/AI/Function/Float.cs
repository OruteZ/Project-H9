using KieranCoppins.DecisionTrees;
using Unity.VisualScripting;
using UnityEngine;

public class Float : H9Function<float>
{
    [SerializeField]
    private float value;
    
    public override float Invoke()
    {
        return value;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get float value";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Get float value";
    }
}