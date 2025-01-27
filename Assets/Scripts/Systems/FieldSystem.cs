using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable InconsistentNaming

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
    
    // AddListener를 쓰라고 public으로 한거지 invoke쓰면 대가리 깨짐
    public static UnityEvent<bool> onCombatFinish => _onCombatFinish ??= new UnityEvent<bool>();


    private static UnityEvent<bool> _onCombatEnter;
    public static UnityEvent<bool> onCombatEnter => _onCombatEnter ??= new UnityEvent<bool>();

    private bool _goalCompleteHandled = false;
    // Game Goal;
    private static IGoal _goal;
    
    private void Awake()
    {
        var initializeBush = BushSystem.Instance;
        
        tileSystem = GetComponent<TileSystem>();
        turnSystem = GetComponent<TurnSystem>();
        unitSystem = GetComponent<UnitSystem>();
        
        _onCombatFinish = new UnityEvent<bool>();
        _onCombatEnter = new UnityEvent<bool>();
        _goalCompleteHandled = false;
    }

    private void Start()
    {
        tileSystem.SetUpTilesAndObjects();
        unitSystem.SetUpUnits();
        turnSystem.SetUp();

        _goal = GameManager.instance.CompareState(GameState.WORLD)
            ? null
            : GoalBuilder.BuildGoal(GameManager.instance.GetStageData().GetGoalInfo());
        
        if (_goal is not null) _goal.AddListenerOnComplete(OnGameGoalComplete);
        else
        {
            if(GameManager.instance.CompareState(GameState.COMBAT)) 
                Debug.LogError("GameState is 'Combat' but Goal is null");
        }
        
        StartCoroutine(StartSceneCoroutine());
    }

    private IEnumerator StartSceneCoroutine()
    {
        Debug.Log("onStageAwake");
        onStageAwake.Invoke();
        if(GameManager.instance.CompareState(GameState.EDITOR) is false)
            UIManager.instance.gameSystemUI.turnUI.SetTurnTextUI();

        yield return new WaitUntil(() => LoadingManager.instance.isLoadingNow is false);

        Debug.Log("onStageStart");
        onStageStart.Invoke();
        if(GameManager.instance.CompareState(GameState.EDITOR) is false) 
            turnSystem.StartTurn();
    }
    
    private void OnGameGoalComplete(bool isWin)
    {
        if (_goalCompleteHandled) return;  // 이미 처리된 경우 무시
        _goalCompleteHandled = true;
        
        if (isWin)
        {
            Debug.Log("Game Goal Complete");
            _onCombatFinish.Invoke(true);
        }
        else
        {
            Debug.Log("Game Goal Failed");
            _onCombatFinish.Invoke(false);
        }
    }

    // ==================== Static Methods ====================
    public static List<IDamageable> GetAllDamageable()
    {
        List<Unit> units = unitSystem.units;
        IEnumerable<TileObject> tileObjs = tileSystem.GetAllTileObjects();
        return units.Concat(tileObjs.OfType<IDamageable>()).ToList();
    }
    
    public static IDamageable GetDamageable(Vector3Int pos)
    {
        IDamageable unit = unitSystem.GetUnit(pos);
        if (unit is not null) return unit;
        
        List<IDamageable> tileObj = tileSystem.GetTileObject(pos).OfType<IDamageable>().ToList();
        return tileObj.Count > 0 ? tileObj[0] : null;
    }
    
    public static bool IsCombatFinish(out bool isWin)
    {
        if (_goal is null)
        {
            isWin = false;
            return false;
        }

        isWin = _goal.HasSuccess();
        return _goal.IsFinished();
    }
    
    public static IGoal GetGoal() => _goal;
}
