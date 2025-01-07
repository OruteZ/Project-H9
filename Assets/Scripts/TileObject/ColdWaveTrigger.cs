using UnityEngine;

public class ColdWaveTrigger : TileObject
{
    [SerializeField] private bool enableTrigger = true;
    
    public override void OnCollision(Unit unit)
    {
        if(unit is not Player) return;

        ColdWave.isEnable = enableTrigger;
        if (enableTrigger)
        {
            ColdWave.SetStartTurn(FieldSystem.turnSystem.GetTurnNumber());
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        
        if(GameManager.instance.CompareState(GameState.COMBAT))
        {
            throw new System.Exception("ColdWaveTrigger is not allowed in combat state");
        }
    }

    public override string[] GetArgs()
    {
        throw new System.NotImplementedException();
    }

    public override void SetArgs(string[] args)
    {
        throw new System.NotImplementedException();
    }
}