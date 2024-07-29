using KieranCoppins.DecisionTrees;
using UnityEngine;

public class FloatCompare : Function<bool>
{
    public ComparisonType comparisonType;
    
    [HideInInspector, SerializeField] private Function<float> value;
    [SerializeField, HideInInspector] private Function<float> compareWith;
    
    public FloatCompare(Function<float> value, Function<float> compareWith)
    {
        this.value = value;
        this.compareWith = compareWith;
    }
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        value.Initialise(metaData);
        compareWith.Initialise(metaData);
    }
    
    public bool Decide()
    {
        return comparisonType switch
        {
            ComparisonType.GREATER_THAN => value.Invoke() > compareWith.Invoke(),
            ComparisonType.GREATER_THAN_OR_EQUAL_TO => value.Invoke() >= compareWith.Invoke(),
            ComparisonType.LESS_THAN => value.Invoke() < compareWith.Invoke(),
            ComparisonType.LESS_THAN_OR_EQUAL_TO => value.Invoke() <= compareWith.Invoke(),
            ComparisonType.EQUAL_TO => value.Invoke() == compareWith.Invoke(),
            _ => false
        };
    }
    
    public override bool Invoke()
    {
        return Decide();
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if input is equal to compare value";
    }
}


    
public enum ComparisonType
{
    GREATER_THAN,
    GREATER_THAN_OR_EQUAL_TO,
    LESS_THAN,
    LESS_THAN_OR_EQUAL_TO,
    EQUAL_TO
}