using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CombatEncounter : TileObject
{
    private bool IsEncounterEnable()
    {
        int curTurn = FieldSystem.turnSystem.turnNumber;
        bool hasFinished = EncounterManager.instance.TryGetTurn(hexPosition, out int lastTurn);

        if (hasFinished is false) return true;
        return lastTurn + 5 <= curTurn;
    }
    public override void OnCollision(Unit other)
    {
        if (IsEncounterEnable() is false) return;
        
        Debug.Log("On Collision Calls");
        EncounterManager.instance.AddValue(hexPosition, FieldSystem.turnSystem.turnNumber);
        GameManager.instance.StartCombat("CombatScene");
    }

    public override void SetVisible(bool value)
    {
        meshRenderer.enabled = value && IsEncounterEnable();
    }
}