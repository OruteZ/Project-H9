using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem instance { get; private set; }
    
    public TileSystem tileSystem;
    public TurnSystem turnSystem;
    public UnitSystem unitSystem;

    public GameState mapState;

    public bool IsPlayerTurn()
    {
        return unitSystem.GetPlayer() == turnSystem.turnOwner;
    }

    private void Awake()
    {
        instance = this;

        GameManager.instance.ChangeState(mapState);
        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
    }

    private void Start()
    {
        tileSystem.SetUpTilesAndObjects();
        unitSystem.SetUpUnits();
        turnSystem.EndTurn();
    }
}
