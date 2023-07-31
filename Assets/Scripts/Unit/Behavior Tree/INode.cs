using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{
    public enum ENodeState
    {
        Running,
        Success,
        Failure
    }

    ENodeState Evaluate();
}
