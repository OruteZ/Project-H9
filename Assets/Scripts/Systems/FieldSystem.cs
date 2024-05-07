using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FieldSystem : MonoBehaviour
{
    public static TileSystem tileSystem;
    public static TurnSystem turnSystem;
    public static UnitSystem unitSystem;

    /// <summary>
    /// Stage가 로딩되었을때 바로 호출되는 이벤트입니다.
    /// </summary>
    private static UnityEvent _onStageAwake;
    public static UnityEvent onStageAwake => _onStageAwake ??= new UnityEvent();

    /// <summary>
    /// Stage의 Fade-in이 완전히 끝난 후 Invoke되는 이벤트입니다. 
    /// </summary>
    private static UnityEvent _onStageStart;
    public static UnityEvent onStageStart => _onStageStart ??= new UnityEvent();
    
    /// <summary>
    /// CombatScene에서 Stage가 끝났을때 호출되는 이벤트입니다. WorldScene에서 호출되지 않습니다.
    /// </summary>
    private static UnityEvent<bool> _onCombatFinish;
    public static UnityEvent<bool> onCombatFinish => _onCombatFinish ??= new UnityEvent<bool>();


    private static UnityEvent<bool> _onCombatEnter;
    public static UnityEvent<bool> onCombatEnter => _onCombatEnter ??= new UnityEvent<bool>();


    private void Awake()
    {
        Debug.Log("Field System : Awake");
        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
    }

    private void Start()
    {
        tileSystem.SetUpTilesAndObjects();
        unitSystem.SetUpUnits();
        turnSystem.SetUp();
        StartCoroutine(StartSceneCoroutine());
    }

    private IEnumerator StartSceneCoroutine()
    {
        Debug.Log("onStageAwake");
        onStageAwake.Invoke();
        if(GameManager.instance.CompareState(GameState.Editor) is false)
            UIManager.instance.gameSystemUI.turnUI.SetTurnTextUI();

        yield return new WaitUntil(() => LoadingManager.instance.isLoadingNow is false);

        Debug.Log("onStageStart");
        onStageStart.Invoke();
        if(GameManager.instance.CompareState(GameState.Editor) is false) 
            turnSystem.StartTurn();
    }
}
