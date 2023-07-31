using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public sealed class SequenceNode : ScriptableObject, INode
{
    private readonly List<INode> _childNodes;

    public SequenceNode(List<INode> childNodes)
    {
        _childNodes = childNodes;
    }

    public INode.ENodeState Evaluate()
    {
        if (_childNodes == null || _childNodes.Count == 0)
            return INode.ENodeState.Failure;

        for (var i = 0; i < _childNodes.Count; i++)
        {
            var child = _childNodes[i];
            var childEval = child.Evaluate();
            
            if (childEval is INode.ENodeState.Running) return INode.ENodeState.Running;
            if (childEval is INode.ENodeState.Success) continue;
            if (childEval is INode.ENodeState.Failure) return INode.ENodeState.Failure;
        }

        return INode.ENodeState.Success;
    }
    
    public List<INode> GetChildren()
    {
        return _childNodes;
    }
}