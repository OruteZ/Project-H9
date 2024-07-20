using KieranCoppins.DecisionTrees;

public class IsOutOfAmmo : H9Condition
{
    public override DecisionTreeNode GetBranch()
    {
        return ai.GetUnit().weapon.currentAmmo <= 0 ? TrueNode : FalseNode;
    }
}