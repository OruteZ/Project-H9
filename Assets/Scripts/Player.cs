using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
public class Player : Unit
{
    //private State<Player>[] _states;
    private StateMachine<Player> _stateMachine;
    [HideInInspector] public Tile target;

    [Header("Status")]
    public int speed;

    public int actionPoint;
    public override void SetUp(string newName)
    {
        base.SetUp(newName);

        ChangeState(new PlayerState.WaitState());
        
        _stateMachine = new StateMachine<Player>();
        _stateMachine.SetUp(this, new PlayerState.WaitState());
    }
    public override void Updated()
    {
        _stateMachine.Execute();
    }

    public void ChangeState(State<Player> state)
    {
        _stateMachine.ChangeState(state);
    }

    public override void StartTurn()
    {
        actionPoint = 100; //todo : 공식 가져와서 행동력 계산하기
        _stateMachine.ChangeState(new PlayerState.SelectState());
    }

    public int CalculateMobility()
    {
        return speed / 10;
    }
}
