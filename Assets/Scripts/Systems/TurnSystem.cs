using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
    [HideInInspector] public UnityEvent onTurnChanged;
    
    public Unit turnOwner;  
    public int turnNumber;

private void Awake()
    {
        turnNumber = GameManager.instance.CompareState(GameState.World) ? GameManager.instance.worldTurn : 0;
        onTurnChanged.AddListener(() => { turnNumber++;});
        onTurnChanged.AddListener(() => UIManager.instance.timingUI.SetTurnText());
        onTurnChanged.AddListener(() => UIManager.instance.combatUI.startTurnTextUI.SetStartTurnTextUI(turnOwner));
    }

    /// <summary>
    /// 전투를 시작합니다. StartTurn과 구분됩니다.
    /// </summary>
    public void StartCombat()
    {
        StartTurn();
    }

    /// <summary>
    /// Turn을 종료합니다. 만약 CombatScene일 경우 전투가 끝났는지 확인해서 FinishTurnCall을 합니다.
    /// </summary>
    public void EndTurn()
    {
        Debug.Log("Finish Turn Call");
        
        //todo : combat finish check
        
        if (FieldSystem.unitSystem.GetPlayer().IsBusy()) return;
        StartTurn();
    }

    private void StartTurn()
    {
        Debug.Log("start turn");
        
        CalculateTurnOwner();
        turnOwner.StartTurn();
        onTurnChanged.Invoke();
    }
    private void CalculateTurnOwner()
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            turnOwner = FieldSystem.unitSystem.GetPlayer();
        }
        else if (GameManager.instance.CompareState(GameState.Combat))
        {
            const int ORDER_LENGTH = 12;
            
            List<Unit> units = FieldSystem.unitSystem.units;
            List<Unit> turnOrder = new List<Unit>();
            List<int> currentRounds = new List<int>();
            
            for (int i = 0; i < units.Count; i++) 
            {
                currentRounds.Add(units[i].currentRound);
            }
            
            while (turnOrder.Count < ORDER_LENGTH * 2)
            {
                int minOrderValueUnitIndex = 0;
                float minOrderValue = CalculateTurnOrderValue(currentRounds[0], units[0].GetStat().speed);
                for (int i = 0; i < units.Count; i++)
                {
                    float orderValue = CalculateTurnOrderValue(currentRounds[i], units[i].GetStat().speed);
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
            UIManager.instance.timingUI.SetTurnOrderUI(turnOrder);
            turnOwner = turnOrder[0];
        }
    }
    private float CalculateTurnOrderValue(int currentRound, int speed) 
    {
        return (((float)currentRound / speed) * 100.0f);
    }
}


