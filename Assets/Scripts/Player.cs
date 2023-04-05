using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
public class Player : Unit
{
    //private State<Player>[] _states;
    private StateMachine<Unit> _stateMachine;

    public override void Setup(string newName)
    {
        base.Setup(newName);

        ChangeState(new WaitState());
        
        _stateMachine = new StateMachine<Unit>();
        _stateMachine.SetUp(this, new WaitState());
    }
    public override void Updated()
    {
        _stateMachine.Execute();
    }

    public void ChangeState(State<Unit> state)
    {
        _stateMachine.ChangeState(state);
    }

    public override void StartTurn()
    {
        _stateMachine.ChangeState(new SelectState());
    }
}
