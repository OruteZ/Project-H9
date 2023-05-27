using System;
using UnityEngine;

public class CombatEncounter : TileObject
{
    public override void OnCollision(Unit other)
    {
        Debug.Log("On Collision Calls");
        GameManager.instance.ChangeMap("CombatScene");
    }
}