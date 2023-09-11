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

    /// <summary>
    /// Combat Scene 이 로딩되었을때 바로 호출되는 이벤트입니다.
    /// </summary>
    private static UnityEvent _onCombatAwake;
    public static UnityEvent onCombatAwake => _onCombatAwake ??= new UnityEvent();

    /// <summary>
    /// CombatScene의 Fade-in이 완전히 끝난 후 Invoke되는 이벤트입니다. 
    /// </summary>
    private static UnityEvent _onCombatStart;

    public static UnityEvent onCombatStart => _onCombatStart ??= new UnityEvent();
    
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
        onCombatAwake.Invoke();
        unitSystem.GetPlayer().ReloadSight();

        yield return new WaitUntil(() => LoadingManager.instance.isLoadingNow is false);
        
        onCombatStart.Invoke();
        turnSystem.StartTurn();
    }
}
