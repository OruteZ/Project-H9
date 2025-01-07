using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class ColdWave : MonoBehaviour
{
    private static int _coldWaveStartTurn = -1;
    public static bool isEnable = true;

    public int coldWavePeriod = 5;
    public int coldWaveDamage = 5;

    public static void SetStartTurn(int turn)
    {
        if (_coldWaveStartTurn == -1)
        {
            _coldWaveStartTurn = turn;
        }
    }
    
    private void Awake()
    {
        FieldSystem.onStageStart.AddListener(SetListener);
    }
    
    private void SetListener()
    {
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            return;
        }
        
        FieldSystem.turnSystem.onTurnChanged.RemoveListener(OnTurnEnd);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
    }

    private void OnTurnEnd()
    {
        int turnDiff = FieldSystem.turnSystem.GetTurnNumber() - _coldWaveStartTurn;
        if (turnDiff % coldWavePeriod == 0)
        {
            ApplyColdWave();
        }
    }

    private void ApplyColdWave()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        // 방한 아이템을 끼고 있을경우 return
        IEnumerable<IItem> temList = GameManager.instance.playerInventory.GetItems();
        foreach (IItem item in temList)
        {
            if (item.GetData().id == 102)
            {
                return;
            }
        }
        
        Damage context = new (
            coldWaveDamage, 
            coldWaveDamage, 
            Damage.Type.DEFAULT, 
            null, 
            null, 
            player
            );
        
        // get player and call TakeDamage method
        FieldSystem.unitSystem.GetPlayer().TakeDamage(context);
    }
}