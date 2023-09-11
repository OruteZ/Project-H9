using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FieldSystem : MonoBehaviour
{
    private static FieldSystem _instance;
    
    public static TileSystem tileSystem;
    public static TurnSystem turnSystem;
    public static UnitSystem unitSystem;
    
    private void Awake()
    {
        Debug.Log("Field System : Awake");
        _instance = this;

        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
    }

    private void Start()
    {
        tileSystem.SetUpTilesAndObjects();
        unitSystem.SetUpUnits();
        turnSystem.SetUp();
        StartCoroutine(StartCombatCoroutine());
    }

    private IEnumerator StartCombatCoroutine()
    {
        unitSystem.GetPlayer().ReloadSight();

        yield return new WaitUntil(() => LoadingManager.instance.isLoadingNow is false);
        turnSystem.StartTurn();
    }
}
