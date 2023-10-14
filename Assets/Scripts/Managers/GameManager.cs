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
    public List<int> playerPassiveIndexList;
    public List<int> playerActiveIndexList;

    #region LEVEL

    [Header("Level system")] 
    public int level = 1;
    public int curExp = 0;
    private int maxExp => level * 100;
    public void GetExp(int exp)
    {
        curExp += exp;
        while (curExp >= maxExp)
        {
            LevelUp();
        }
        UIManager.instance.onPlayerStatChanged.Invoke();
    }
    private void LevelUp()
    {
        if (maxExp > curExp) return;

        curExp -= maxExp;
        level++;
        playerStat.Recover(StatType.CurHp, playerStat.GetStat(StatType.MaxHp));
        if (level % 3 == 0)
        {
            UIManager.instance.gameSystemUI.playerStatLevelUpUI.OpenPlayerStatLevelUpUI();
        }
    }
    public int GetMaxExp() 
    {
        return maxExp;
    }
    #endregion
    
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

    public void AddPlayerSkillListElement(int skillIndex, bool isPassive) 
    {
        List<int> list = null;
        if (isPassive) 
        {
            list = playerPassiveIndexList;
        }
        else
        {
            list = playerActiveIndexList;
        }
        foreach (int i in list) 
        {
            if (i == skillIndex) 
            {
                Debug.Log("동일한 스킬 연속 습득 오류");
                return;
            }
        }
        list.Add(skillIndex);
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;
        Service.OnUpdated(deltaTime);
    }
}
