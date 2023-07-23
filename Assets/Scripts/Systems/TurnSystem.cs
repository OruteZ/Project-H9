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
        turnNumber = 0;
        onTurnChanged.AddListener(() => { turnNumber++;});
        onTurnChanged.AddListener(() => UIManager.instance.timingUI.SetTurnText(turnNumber));
    }

    /// <summary>
    /// 전투를 시작합니다. StartTurn과 구분됩니다.
    /// </summary>
    public void StartCombat()
    {
        //InitCurrentRound();

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

    private void InitCurrentRound() 
    {
        List<Unit> units = FieldSystem.unitSystem.units;

        foreach (Unit unit in units)
        {
            unit.currentRound = 1;
        }
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
            List<Unit> turnOrder = new List<Unit>();
            List<Unit> units = FieldSystem.unitSystem.units;

            while (turnOrder.Count < ORDER_LENGTH)
            {
                Unit minOrderValueUnit = units[0];
                float minOrderValue = CalculateTurnOrderValue(units[0]);
                foreach (Unit unit in units) 
                {
                    float orderValue = CalculateTurnOrderValue(unit);
                    if (minOrderValue > orderValue) 
                    {
                        minOrderValueUnit = unit;
                        minOrderValue = orderValue;
                    }
                }
                minOrderValueUnit.currentRound += 1;
                turnOrder.Add(minOrderValueUnit);
            }

            UIManager.instance.timingUI.SetTurnOrderUI(turnOrder);
            turnOwner = turnOrder[0];
        }
    }
    private float CalculateTurnOrderValue(Unit unit) 
    {
        return (((float)unit.currentRound / unit.GetStat().speed) * 100.0f);
    }
}


