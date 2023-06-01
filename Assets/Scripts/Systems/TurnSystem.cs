using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class TurnSystem : MonoBehaviour
{
    [HideInInspector] public UnityEvent onTurnChanged;
    
    public Unit turnOwner;  
    public int turnNumber;
    private TurnState _curState;
    
    private void Awake()
    {
        turnNumber = 0;
        onTurnChanged.AddListener(() => { turnNumber++;});
        _curState = TurnState.NotStarted;
    }

    private void Update()
    {
        if (_curState == TurnState.PassedInitiative) return;
        if (_curState == TurnState.NotStarted) return;
        if (_curState == TurnState.GetInitiative)
        {
            StartTurn();
        }
        
    }

    private void StartGame()
    {
        #if UNITY_EDITOR
        if (_curState == TurnState.NotStarted)
        {
            Debug.LogError("Turn 2회 시작");
            return;
        }
        #endif
        _curState = TurnState.GetInitiative;
    }

    public void EndTurn()
    {
        //todo : if combat has finished, End Combat Scene
        //else

        Debug.Log("FinishTurn Call");
        _curState = TurnState.GetInitiative;
    }

    private void StartTurn()
    {
        CalculateTurnOwner();
        turnOwner.StartTurn();
        onTurnChanged.Invoke();

        _curState = TurnState.PassedInitiative;
    }

    private void CalculateTurnOwner()
    {
        if (GameManager.instance.currentState == GameState.World)
        {
            turnOwner = CombatSystem.instance.unitSystem.GetPlayer();
        }
        
        var units = CombatSystem.instance.unitSystem.units;

        int total = 0;

        List<int> values = new List<int>();
        values.Add(0);
        
        for(int i = 1; i < units.Count + 1; i++)
        {
            var unit = units[i - 1];
            
            //누적합 생성
            values.Add(unit.stat.speed);
            total += values[i];
            values[i] += values[i - 1];

        }

        foreach (var v in values)
        {
            Debug.Log(v);
        }

        int result = UnityEngine.Random.Range(0, total);
        for (int i = 1; i < values.Count; i++)
        {
            if (values[i - 1] <= result && result < values[i])
            {
                turnOwner = units[i - 1];
                return;
            }
        }
        
        Debug.LogError("Error result is " + result);

    
        //turnOwner = CombatSystem.instance.unitSystem.GetPlayer();
    }
}

internal enum TurnState
{
    NotStarted,
    GetInitiative,
    SelectingNextTurn, //추후 턴 선택시 바로 시작 전 이펙트 표현을 위해 추가한 State
    PassedInitiative,
}


