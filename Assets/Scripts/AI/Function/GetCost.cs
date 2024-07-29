using KieranCoppins.DecisionTrees;

public class GetCost : H9Function<float>
{
    public override float Invoke()
    {
        return ai.GetUnit().stat.curActionPoint;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get current action point";
    }
}