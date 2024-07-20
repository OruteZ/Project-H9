using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetUnitAction : H9Function<IUnitAction>
{
    [SerializeField]
    private ActionType actionType;
    
    public override IUnitAction Invoke()
    {
        return ai.GetUnit().GetAction(actionType);
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get Unit Action : " + actionType.ToString();
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Get Unit Action : " + actionType.ToString();
    }
}