using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public TileSystem tileSystem;
    public TurnSystem turnSystem;
    public UnitSystem unitSystem;

    public bool IsPlayerTurn()
    {
        return unitSystem.GetPlayer() == turnSystem.turnOwner;
    }

    private void Awake()
    {
        Instance = this;

        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
    }
}
