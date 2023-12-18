using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Link : TileObject
{
    public int linkIndex;
    public int combatMapIndex;
    
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

        other.GetSelectedAction().ForceFinish();
        
        Debug.Log("On Collision Calls");
        EncounterManager.instance.AddValue(hexPosition, FieldSystem.turnSystem.turnNumber);
        GameManager.instance.StartCombat(combatMapIndex, linkIndex: linkIndex);
    }

    public override void SetVisible(bool value)
    {
        meshRenderer.enabled = value && IsEncounterEnable();
    }

    public override string[] GetArgs()
    {
        return new [] { linkIndex.ToString() };
    }

    public override void SetArgs(string[] args)
    {
        linkIndex = int.Parse(args[0]);
    }
}