using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Combat,
    World,
    Editor
}
public class GameManager : Generic.Singleton<GameManager>
{
    private static string COMBAT_SCENE_NAME = "CombatScene";
    
    private HashSet<Vector3Int> _pioneeredWorldTileSet;
    
    [SerializeField]
    private GameState _currentState = GameState.World;

    [SerializeField] private CombatStageData _stageData;
    [SerializeField]
    private int _currentLinkIndex = -1;

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
    public void StartCombat(int stageIndex, int linkIndex)
    {
        //Save World Data
        worldAp = FieldSystem.unitSystem.GetPlayer().currentActionPoint;
        worldTurn = FieldSystem.turnSystem.turnNumber;

        playerWorldPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
        
        ChangeState(GameState.Combat);
        _currentLinkIndex = linkIndex;
        _stageData = Resources.Load<CombatStageData>($"Map Data/Stage {stageIndex}");
        LoadingManager.instance.LoadingScene(COMBAT_SCENE_NAME);
    }
    
    public int GetLinkIndex()
    {
        return _currentLinkIndex;
    }
    
    public CombatStageData GetStageData()
    {
        return _stageData;
    }

    public void FinishCombat()
    {
        ChangeState(GameState.World);

        backToWorldTrigger = true;
        LoadingManager.instance.LoadingScene(worldSceneName);
    }

    public void SetEditor()
    {
        ChangeState(GameState.Editor);
    }

    public bool CompareState(GameState state)
    {
        return _currentState == state;
    }

    private void ChangeState(GameState state)
    {
        if (CompareState(state)) return;

        _currentState = state;
        //UIManager.instance.ChangeScenePrepare(state);
    }

    public void AddPlayerSkillListElement(SkillInfo skillInfo)
    {
        List<int> list = null;
        if (skillInfo.IsPassive())
        {
            list = playerPassiveIndexList;
        }
        else
        {
            list = playerActiveIndexList;
        }
        if (list.IndexOf(skillInfo.index) != -1)
        {
            Debug.Log("동일한 스킬 연속 습득 오류");
            return;
        }

        int USIPosition = -1;
        if (skillInfo.IsActive())
        {
            ActiveInfo aInfo = SkillManager.instance.activeDB.GetActiveInfo(skillInfo.index);
            int upgSkillIndex = SkillManager.instance.FindUpgradeSkillIndex(skillInfo, aInfo.action);
            USIPosition = list.IndexOf(upgSkillIndex);
        }

        if (USIPosition == -1)
        {
            list.Add(skillInfo.index);
        }
        else
        {
            list.RemoveAt(USIPosition);
            list.Insert(USIPosition, skillInfo.index);
        }
    }
    
    public bool IsPioneeredWorldTile(Vector3Int tilePos)
    {
        return _pioneeredWorldTileSet.Contains(tilePos);
    }

    public void AddPioneeredWorldTile(Vector3Int tilePos)
    {
        if (_pioneeredWorldTileSet.Contains(tilePos)) return;
        
        _pioneeredWorldTileSet.Add(tilePos);
    }
    
    private new void Awake()
    {
        base.Awake();
        
        _pioneeredWorldTileSet = new ();
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;
        Service.OnUpdated(deltaTime);
    }
}
