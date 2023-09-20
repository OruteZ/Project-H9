using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectorNode", menuName = "ScriptableObjects/BT/Selector", order = 1)]
public class ActionNode : ScriptableObject, INode
{
    private readonly Func<INode.ENodeState> _onEvaluate = null;

    public ActionNode(Func<INode.ENodeState> onEvaluate)
    {
        _onEvaluate = onEvaluate;
    }

    public INode.ENodeState Evaluate()
    {
        return _onEvaluate?.Invoke() ?? INode.ENodeState.Failure;
    }

    public Func<INode.ENodeState> GetFunc()
    {
        return _onEvaluate;
    }
}
