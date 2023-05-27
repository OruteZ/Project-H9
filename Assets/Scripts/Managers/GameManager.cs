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
    public GameState currentState;

    [Header("Player Info")] 
    public Vector3Int playerWorldPos;
    public UnitStat playerStat;
    public Weapon playerWeapon;

    public void ChangeState(GameState state)
    {
        if (currentState == state) return;

        currentState = state;
    }

    public void ChangeMap(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
