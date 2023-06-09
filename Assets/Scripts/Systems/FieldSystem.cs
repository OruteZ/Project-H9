using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FieldSystem : MonoBehaviour
{
    private FieldSystem _instance;
    
    public static TileSystem tileSystem;
    public static TurnSystem turnSystem;
    public static UnitSystem unitSystem;
    
    private void Awake()
    {
        _instance = this;

        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
        
        unitSystem.SetUpUnits();
        tileSystem.SetUpTilesAndObjects();
    }

    private void Start()
    {
        turnSystem.StartCombat();
    }
}
