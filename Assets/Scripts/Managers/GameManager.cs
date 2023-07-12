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
    private GameState _currentState = GameState.World;

    [Header("Player Info")] 
    public Vector3Int playerWorldPos;
    public UnitStat playerStat;
    public Weapon playerWeapon = null;

    [Header("World Scene Name")] 
    public string worldSceneName;

    public void StartCombat(string combatSceneName)
    {
        playerWorldPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
        SceneManager.LoadScene(combatSceneName);
        ChangeState(GameState.Combat);
    }

    public void FinishCombat()
    {
        SceneManager.LoadScene(worldSceneName);
        ChangeState(GameState.World);
    }

    public bool CompareState(GameState state)
    {
        return _currentState == state;
    }
    
    private void ChangeState(GameState state)
    {
        if(CompareState(state)) return;

        _currentState = state;
    }
}
