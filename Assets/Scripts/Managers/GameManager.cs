using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DGS = System.Diagnostics;

public enum GameState
{
    Combat,
    World,
    Editor
}
public class GameManager : Generic.Singleton<GameManager>
{
    private const string COMBAT_SCENE_NAME = "CombatScene";

    public Inventory playerInventory = new Inventory();
    [SerializeField]
    public ItemDatabase itemDatabase;
    public WeaponDatabase weaponDatabase;
    public List<QuestInfo> Quests;
    
    #region ITEM_TEST
    public void AddItem(int id)
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not initialized");
            return;
        }

        ItemData itemData = itemDatabase.GetItemData(id);

        if (itemData == null)
        {
            Debug.LogError($"No item data found for id {id}");
            return;
        }

        Item item = Item.CreateItem(itemData);

        playerInventory.TryAddItem(item);

        Debug.Log("Added item to inventory");
    }//a
    #endregion

    private HashSet<Vector3Int> _discoveredWorldTileSet;
    
    [SerializeField]
    private GameState _currentState = GameState.World;

    [SerializeField] private CombatStageData _stageData;
    [SerializeField]
    private int _currentLinkIndex = -1;

    [Header("Player Info")]
    public Vector3Int playerWorldPos;
    public UnitStat playerStat;
    [SerializeField] private int _playerWeaponIndex;
    public int PlayerWeaponIndex
    {
        get => _playerWeaponIndex;
        set
        {
            _playerWeaponIndex = value;
            Weapon weapon = weaponDatabase.Clone(value);
            onPlayerWeaponChanged.Invoke(weapon); 
        }
    }
    public GameObject playerModel;
    public List<int> playerPassiveIndexList;
    public List<int> playerActiveIndexList;
    
    public UnityEvent<Weapon> onPlayerWeaponChanged = new UnityEvent<Weapon>(); // �̰� �� ������
    public UnityEvent<int> onPlayerCombatFinished = new UnityEvent<int>(); // <LinkIndex>, Combat manager ��ũ��Ʈ�� Player �׼� ��ũ��Ʈ(not player data)�� ������ �ű�����

    #region LEVEL

    [Header("Level system")]
    public int level = 1;
    public int curExp = 0;
    private int maxExp => level * 100;
    private const int LEVEL_UP_REWARD_SKILL_POINT = 1;
    public void GetExp(int exp)
    {
        curExp += exp;
        while (curExp >= maxExp)
        {
            LevelUp();
        }
        UIManager.instance.onPlayerStatChanged.Invoke();
        UIManager.instance.onGetExp.Invoke(exp);
    }
    private void LevelUp()
    {
        if (maxExp > curExp) return;

        curExp -= maxExp;
        level++;
        playerStat.Recover(StatType.CurHp, playerStat.GetStat(StatType.MaxHp), out var appliedValue);
        SkillManager.instance.AddSkillPoint(LEVEL_UP_REWARD_SKILL_POINT);
        if (level % 3 == 0)
        {
            UIManager.instance.gameSystemUI.playerStatLevelUpUI.OpenPlayerStatLevelUpUI();
            UIManager.instance.onLevelUp.Invoke(level);
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

    private UnityEvent OnGameStarted = new UnityEvent();
    private UnityEvent<QuestInfo> OnNotifiedQuestEnd = new UnityEvent<QuestInfo>();
    private UnityEvent<QuestInfo> OnNotifiedQuestStart = new UnityEvent<QuestInfo>();

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
        onPlayerCombatFinished?.Invoke(GetLinkIndex());
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

        int previousSkillPositionIndex = -1;
        if (skillInfo.IsActive())
        {
            ActionType addedActionType = SkillManager.instance.activeDB.GetActiveInfo(skillInfo.index).action;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                ActionType actionType = SkillManager.instance.activeDB.GetActiveInfo(list[i]).action;
                if (addedActionType == actionType) 
                {
                    list.RemoveAt(i);
                    previousSkillPositionIndex = i;
                }
            }
        }
        if (previousSkillPositionIndex == -1)
        {
            list.Add(skillInfo.index);
        }
        else
        {
            list.Insert(previousSkillPositionIndex, skillInfo.index);
        }
    }
    
    public bool IsPioneeredWorldTile(Vector3Int tilePos)
    {
        return _discoveredWorldTileSet.Contains(tilePos);
    }
    
    public List<Vector3Int> GetPioneeredWorldTileList()
    {
        return new List<Vector3Int>(_discoveredWorldTileSet);
    }

    public void AddPioneeredWorldTile(Vector3Int tilePos)
    {
        if (_discoveredWorldTileSet.Contains(tilePos)) return;
        
        _discoveredWorldTileSet.Add(tilePos);
    }
    
    private new void Awake()
    {
        base.Awake();
        
        _discoveredWorldTileSet = new ();
        var watch = DGS.Stopwatch.StartNew();
        var qi = new QuestParser();
        Quests = qi.GetQuests();
        watch.Stop();
        Debug.Log($"Quest parse time: {watch.ElapsedMilliseconds}");

    }

    private void Start()
    {
        #region ����Ʈ ����. ���߿� �ű� ����
        var watch = DGS.Stopwatch.StartNew();
        foreach (var quest in Quests)
        {
            // ���� ���۽� �����ϴ� ����Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GAME_START))
                OnGameStarted.AddListener(quest.OnOccurConditionEvented);

            // ����Ʈ ���۽� Invoke �Լ� ȣ��
            quest.OnQuestStarted.AddListener(InvokeQuestStart);
            // ����Ʈ �Ϸ�� Invoke �Լ� ȣ��
            quest.OnQuestEnded.AddListener(InvokeQuestEnd);

            // ����Ʈ �Ϸ�� �����ϴ� ����Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedConditionEvented(q.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedGoalEvented(q.Index));

            // ����Ʈ ����, �Ϸ��� MOVE_TO ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedConditionEvented(pos));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedGoalEvented(pos));

            // ����Ʈ ����, �Ϸ���� KILL_LINK ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountConditionEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountGoalEvented);
             
            // ����Ʈ ����, �Ϸ���� KILL_UNIT ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u)=>quest.OnCountConditionEvented(u.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u)=>quest.OnCountConditionEvented(u.Index));

            // ����Ʈ ����, �Ϸ���� GET_ITEM, USE_TIEM ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener(quest.OnCountConditionEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener(quest.OnCountGoalEvented);

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener(quest.OnCountConditionEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener(quest.OnCountGoalEvented);
            
            // �÷��̾� �þ� ���� Ÿ��, ��ũ ���Խ��� �̺�Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedConditionEvented(tile.hexPosition));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedGoalEvented(tile.hexPosition));
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedConditionEvented(link.linkIndex));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedGoalEvented(link.linkIndex));
            
            if (quest.ExpireTurn != -1)
                UIManager.instance.onTurnStarted.AddListener((u) => { if (u is Player) quest.ProgressExpireTurn();});
        }
        watch.Stop();
        Debug.Log($"Quest link time: {watch.ElapsedMilliseconds}");

        OnNotifiedQuestStart.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.StartConversation(q.StartConversation); });
        OnNotifiedQuestStart.AddListener((q) => { UIManager.instance.gameSystemUI.questUI.AddQuestListUI(q); });
        OnNotifiedQuestEnd.AddListener((q) => { UIManager.instance.gameSystemUI.questUI.DeleteQuestListUI(q.Index); });
        #endregion

        OnGameStarted?.Invoke();
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;
        Service.OnUpdated(deltaTime);
        
        #region ITEM_TEST

        if (Input.GetKeyDown(KeyCode.F1))
        {
            //add all item
            var allItems = itemDatabase.GetAllItemData();

            for (int i = 1; i < allItems.Capacity; i++)
            {
                ItemData itemData = allItems[i];
                //Debug.Log(itemData.id);
                AddItem(itemData.id);
            }
        }
        #endregion
    }

    private void InvokeQuestEnd(QuestInfo quest)
    {
        OnNotifiedQuestEnd?.Invoke(quest);
    }

    private void InvokeQuestStart(QuestInfo quest)
    {
        OnNotifiedQuestStart?.Invoke(quest);
    }
}
