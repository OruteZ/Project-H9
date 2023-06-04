using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MainSystem : MonoBehaviour
{
    public static MainSystem instance { get; private set; }
    public TileSystem tileSystem;
    public TurnSystem turnSystem;
    public UnitSystem unitSystem;

    public GameState mapState;
    
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

    public bool IsPlayerTurn()
    {
        return unitSystem.GetPlayer() == turnSystem.turnOwner;
    }

    
}
