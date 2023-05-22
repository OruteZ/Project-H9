using System.Collections;
using System.Collections.Generic;
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
    }
    
    private void Start()
    {
        EndTurn();
    }

    public void EndTurn()
    {
        //todo : if combat has finished, End Combat Scene
        //else

        CalculateTurnOwner();
        StartTurn();
    }
    
    public void StartTurn()
    {
        turnOwner.StartTurn();
        onTurnChanged.Invoke();
    }

    private void CalculateTurnOwner()
    {
        turnOwner = CombatManager.Instance.unitSystem.GetPlayer();
    }
}
