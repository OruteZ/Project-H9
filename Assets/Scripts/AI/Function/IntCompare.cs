using KieranCoppins.DecisionTrees;
using UnityEngine;

public class IntCompare : Function<bool>
{
    [SerializeField] 
    private int input;
    
    [SerializeField]
    private int compareValue;
    
    public override bool Invoke()
    {
        return input == compareValue;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if input is equal to compare value";
    }
}