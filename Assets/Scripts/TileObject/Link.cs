using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Link : TileObject
{
    public int linkIndex;
    public int combatMapIndex;

    private bool _vision;
    
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
        _vision = value;
    }

    public override string[] GetArgs()
    {
        return new [] { linkIndex.ToString() };
    }

    public override void SetArgs(string[] args)
    {
        linkIndex = int.Parse(args[0]);
    }
    
    public override void SetUp()
    {
        base.SetUp();
        
        //Link는 World Object라서 한번 밝혀지면 상관이 없지만 
        //턴이 바뀜에 따라서 안보이는게 보일 수 있으니 확인 해줘야 함
        FieldSystem.turnSystem.onTurnChanged.AddListener(() => SetVisible(_vision));
    }
}