using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Combat,
    World
}
public class GameManager : Generic.Singleton<GameManager>
{
    [SerializeField]
    private GameState _currentState = GameState.World;

    [Header("Player Info")] 
    public Vector3Int playerWorldPos;
    public UnitStat playerStat;
    public int playerWeaponIndex;
    public GameObject playerModel;

    [Header("World Scene Name")] 
    public string worldSceneName;

    [Header("World Info")] 
    public int worldAp;
    public int worldTurn;

    public bool backToWorldTrigger = false;
    public void StartCombat(string combatSceneName)
    {
        //Save World Data
        worldAp = FieldSystem.unitSystem.GetPlayer().currentActionPoint;
        worldTurn = FieldSystem.turnSystem.turnNumber;

        playerWorldPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
        //SceneManager.LoadScene(combatSceneName);
        ChangeState(GameState.Combat);
        LoadingManager.instance.LoadingScene(combatSceneName);
    }

    public void FinishCombat()
    {
        ChangeState(GameState.World);

        backToWorldTrigger = true;
        LoadingManager.instance.LoadingScene(worldSceneName);
    }

    public bool CompareState(GameState state)
    {
        return _currentState == state;
    }
    
    private void ChangeState(GameState state)
    {
        if(CompareState(state)) return;

        _currentState = state;
        //UIManager.instance.ChangeScenePrepare(state);
    }
}
