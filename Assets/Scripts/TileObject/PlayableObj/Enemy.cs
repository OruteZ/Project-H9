using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public override void Updated()
    {
        return;
    }

    public override void StartTurn()
    {
        TurnSystem.EndTurn();
    }

    public override void OnHit(int damage)
    {
        Debug.Log(damage + " 데미지 받음");
    }
}
