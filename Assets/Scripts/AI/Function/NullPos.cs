using KieranCoppins.DecisionTrees;
using UnityEngine;

public class NullPos : H9Function<Vector3Int>
{
    public override Vector3Int Invoke()
    {
        return Vector3Int.zero;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Return null position";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Return null position";
    }
}