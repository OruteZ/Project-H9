using Generic;
using UnityEngine;


public class WaitState : State<Unit>
{
    public override void Enter(Unit entity)
    {
        throw new System.NotImplementedException();
    }
    public override void Execute(Unit entity)
    {
        throw new System.NotImplementedException();
    }
    public override void Exit(Unit entity)
    {
        throw new System.NotImplementedException();
    }
}

public class SelectState : State<Unit>
{
    public override void Enter(Unit entity)
    {
        throw new System.NotImplementedException();
    }
    public override void Execute(Unit entity)
    {
        throw new System.NotImplementedException();
    }
    public override void Exit(Unit entity)
    {
        throw new System.NotImplementedException();
    }
}

public class Move : State<Unit>
{
    private const float OneTileMoveTime = 0.2f;
    
    public override void Enter(Unit entity)
    {
        Debug.Log("Current State = Move");
    }
    public override void Execute(Unit entity)
    {
        throw new System.NotImplementedException();
    }
    public override void Exit(Unit entity)
    {
        throw new System.NotImplementedException();
    }
}
