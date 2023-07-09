using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Combat,
    World
}
public class GameManager : Generic.Singleton<GameManager>
{
    private GameState _currentState;

    [Header("Player Info")] 
    public Vector3Int playerWorldPos;
    public UnitStat playerStat;
    public Weapon playerWeapon;

    [Header("World Scene Name")] 
    public string worldSceneName; 

    public void ChangeState(GameState state)
    {
        if(CompareState(state)) return;

        _currentState = state;
    }

    private void ChangeMap(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartCombat(string combatSceneName)
    {
        playerWorldPos = CombatSystem.instance.unitSystem.GetPlayer().hexPosition;
        ChangeMap(combatSceneName);
        ChangeState(GameState.Combat);
    }

    public void FinishCombat()
    {
        ChangeMap(worldSceneName);
        ChangeState(GameState.World);
    }

    public bool CompareState(GameState state)
    {
        return _currentState == state;
    }
}
