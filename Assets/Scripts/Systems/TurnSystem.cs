using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
    [HideInInspector] public UnityEvent onTurnChanged;
    
    public Unit turnOwner;

    public int turnNumber => GetTurnNumber();
    [SerializeField] private int _combatTurnNumber;
    [SerializeField] private int _worldTurnNumber;

    public int GetTurnNumber()
    {
        // compare state
        if (GameManager.instance.CompareState(GameState.WORLD))
        {
            return _worldTurnNumber;
        }
        else
        {
            return _combatTurnNumber;
        }
    }

    public int GetTurnNumber(GameState state)
    {
        if (state == GameState.WORLD)
        {
            return _worldTurnNumber;
        }
        else
        {
            return _combatTurnNumber;
        }
    }

    private void Start()
    {
        _worldTurnNumber = GameManager.instance.runtimeWorldData.worldTurn;
        _combatTurnNumber = 0;
        
        onTurnChanged.AddListener(() => UIManager.instance.gameSystemUI.turnUI.SetTurnTextUI());
        // onTurnChanged.AddListener(() => UIManager.instance.combatUI.startTurnTextUI.SetStartTurnTextUI(turnOwner));
        onTurnChanged.AddListener(() => GameManager.instance.runtimeWorldData.worldTurn = _worldTurnNumber);
        onTurnChanged.AddListener(() => UIManager.instance.combatUI.combatPopupTextUI.SetStartTurnTextUI(turnOwner));
    }

    public void SetUp()
    {
       CalculateTurnOwner();

       if (GameManager.instance.CompareState(GameState.WORLD))
       {
           Player player = FieldSystem.unitSystem.GetPlayer();
           player.onMoved.AddListener((from, to, a) =>
           {
               _worldTurnNumber++;
               onTurnChanged.Invoke();
           });
       }
    }

    /// <summary>
    /// Turn을 종료합니다. 만약 CombatScene일 경우 전투가 끝났는지 확인해서 FinishTurnCall을 합니다.
    /// </summary>
    public void EndTurn()
    {
        // var player = FieldSystem.unitSystem.GetPlayer();
        // if (player is not null)
        // {
        //     if (FieldSystem.unitSystem.GetPlayer().IsBusy()) return;
        // }
        //
        // else {
        //     Debug.Log($"Player is null, Turn system is rest.");
        //     return;
        // }
        
        // turnOwner.animator.ResetTrigger("Idle");
        
        CalculateTurnOwner();
        StartTurn();
    }

    public void StartTurn()
    {
        if (GameManager.instance.backToWorldTrigger)
        {
            GameManager.instance.backToWorldTrigger = false;
            ((Player)turnOwner).ContinueWorldTurn();
        }
        else
        {
            if (GameManager.instance.CompareState(GameState.COMBAT)) _combatTurnNumber++;
            
            onTurnChanged.Invoke(); // 유닛의 n번째 턴이 시작되기 위해 n번째 턴을 먼저 만들고 나서, 유닛의 StartTurn()
            turnOwner.StartTurn();
            GameManager.instance.Save(true);
        }
    }
    private void CalculateTurnOwner()
    {
        if (GameManager.instance.CompareState(GameState.COMBAT) is false)
        {
            turnOwner = FieldSystem.unitSystem.GetPlayer();
        }
        else
        {
            const int ORDER_LENGTH = 12;
            
            List<Unit> units = FieldSystem.unitSystem.units;
            List<Unit> turnOrder = new List<Unit>();
            List<int> currentRounds = units.Select(t => t.currentRound).ToList();

            while (turnOrder.Count < ORDER_LENGTH * 4)
            {
                int minOrderValueUnitIndex = 0;
                float minOrderValue = CalculateTurnOrderValue(currentRounds[0], units[0].stat.GetStat(StatType.Speed));
                for (int i = 0; i < units.Count; i++)
                {
                    float orderValue = CalculateTurnOrderValue(currentRounds[i], units[i].stat.GetStat(StatType.Speed));
                    if (minOrderValue > orderValue)
                    {
                        minOrderValueUnitIndex = i;
                        minOrderValue = orderValue;
                    }
                }
                currentRounds[minOrderValueUnitIndex]++;
                turnOrder.Add(units[minOrderValueUnitIndex]);
            }

            turnOrder[0].currentRound++;
            UIManager.instance.onStartedCombatTurn.Invoke(turnOrder[0]);
            UIManager.instance.combatUI.turnOrderUI.SetTurnOrderUI(turnOrder);
            turnOwner = turnOrder[0];
        }
    }
    private float CalculateTurnOrderValue(int currentRound, int speed) 
    {
        return (((float)currentRound / speed) * 100.0f);
    }
}


