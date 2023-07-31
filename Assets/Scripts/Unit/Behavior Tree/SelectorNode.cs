using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectorNode : ScriptableObject, INode
{
    [SerializeField]
    private readonly List<INode> _childNodes;

    public SelectorNode(List<INode> childNodes)
    {
        _childNodes = childNodes;
    }
    
    public INode.ENodeState Evaluate()
    {
        if (_childNodes == null)
            return INode.ENodeState.Failure;

        for (var i = 0; i < _childNodes.Count; i++)
        {
            var child = _childNodes[i];
            var childEval = child.Evaluate();

            if (childEval is INode.ENodeState.Success) return INode.ENodeState.Success;
            if (childEval is INode.ENodeState.Running) return INode.ENodeState.Running;
        }

        return INode.ENodeState.Failure;
    }

    public List<INode> GetChildren()
    {
        return _childNodes;
    }
}
