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
        turnOwner = FieldSystem.unitSystem.GetPlayer();
    }
}


