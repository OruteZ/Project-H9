using KieranCoppins.DecisionTrees;
using UnityEngine;

public class Compare : H9Condition
{
    public enum ComparisonType
    {
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL_TO,
        LESS_THAN,
        LESS_THAN_OR_EQUAL_TO,
        EQUAL_TO
    }
    
    public ComparisonType comparisonType;
    
    [HideInInspector][SerializeField] private Function<float> value;
    [SerializeField] private float compareWith;
    
    public Compare(Function<float> value)
    {
        this.value = value;
    }
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        value.Initialise(metaData);
    }
    
    public bool Decide()
    {
        return comparisonType switch
        {
            ComparisonType.GREATER_THAN => value.Invoke() > compareWith,
            ComparisonType.LESS_THAN => value.Invoke() < compareWith,
            ComparisonType.EQUAL_TO => value.Invoke() == compareWith,
            ComparisonType.GREATER_THAN_OR_EQUAL_TO => value.Invoke() >= compareWith,
            ComparisonType.LESS_THAN_OR_EQUAL_TO => value.Invoke() <= compareWith,
            _ => false
        };
    }
    
    public override DecisionTreeNode GetBranch()
    {
        return Decide() ? TrueNode : FalseNode;
    }
}