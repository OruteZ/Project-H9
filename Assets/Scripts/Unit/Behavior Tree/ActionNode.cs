using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ActionNode : INode
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
