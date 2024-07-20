using KieranCoppins.DecisionTrees;
using UnityEngine;
using UnityEngine.Serialization;

public class IsExecutable : H9Condition
{
    [HideInInspector] [SerializeField] protected Function<Vector3Int> target;
    [HideInInspector] [SerializeField] protected Function<IUnitAction> action;
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        action.Initialise(metaData);
        target.Initialise(metaData);
    }

    public IsExecutable(Function<IUnitAction> action, Function<Vector3Int> target)
    {
        this.action = action;
        this.target = target;
    }
    
    public override DecisionTreeNode GetBranch()
    {
        IUnitAction action = this.action.Invoke();
        
        Vector3Int targetPos = Vector3Int.zero;
        if(targetPos != null)  targetPos = this.target.Invoke();
        
        if (action is null) return FalseNode;
        if (action.IsSelectable() is false) return FalseNode;
        if (action.CanExecute(targetPos) is false) return FalseNode;
        
        return TrueNode;
    }
}