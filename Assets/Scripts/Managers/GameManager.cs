using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    
    //databases
    public ItemDatabase itemDatabase;
    public WeaponDatabase weaponDatabase;
    
    // map data
    public WorldData runtimeWorldData;
    [SerializeField] private WorldData _defaultWorldData;

    public Inventory playerInventory = new Inventory();
    public List<QuestInfo> Quests;
    
    [SerializeField]
    private GameState _currentState = GameState.World;

    [SerializeField] private CombatStageData _stageData;
    [SerializeField] private int _currentLinkIndex = -1;

    [Header("Player Info")]
    public UnitStat playerStat;
    [FormerlySerializedAs("_playerWeaponIndex")] public int playerWeaponIndex;
    public GameObject playerModel;
    public List<int> playerPassiveIndexList;
    public List<int> playerActiveIndexList;
    
    public UnityEvent<Weapon> onPlayerWeaponChanged = new UnityEvent<Weapon>(); // �̰� �� ������
    public UnityEvent<int> onPlayerCombatFinished = new UnityEvent<int>(); // <LinkIndex>, Combat manager ��ũ��Ʈ�� Player �׼� ��ũ��Ʈ(not player data)�� ������ �ű�����

    
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
            UIManager.instance.gameSystemUI.playerStatLevelUpUI.GetPlayerStatPoint();
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

    public bool backToWorldTrigger = false;

    private UnityEvent OnGameStarted = new UnityEvent();
    private UnityEvent<QuestInfo> OnNotifiedQuestEnd = new UnityEvent<QuestInfo>();
    private UnityEvent<QuestInfo> OnNotifiedQuestStart = new UnityEvent<QuestInfo>();

    private void SaveCurrentWorldData()
    {
        worldAp = FieldSystem.unitSystem.GetPlayer().currentActionPoint;
        runtimeWorldData.worldTurn = FieldSystem.turnSystem.turnNumber;

        runtimeWorldData.playerPosition = FieldSystem.unitSystem.GetPlayer().hexPosition;
        
        //save links
        runtimeWorldData.links = new List<LinkObjectData>();
        
        foreach (Link link in FieldSystem.tileSystem.GetAllTileObjects().Where(obj => obj is Link))
        {
            LinkObjectData linkData = new LinkObjectData();
            linkData.pos = link.hexPosition;
            linkData.linkIndex = link.linkIndex;
            linkData.combatMapIndex = link.combatMapIndex;
            linkData.isRepeatable = link.isRepeatable;
            // linkData.modelName = link.;
            // The Link Model is one-to-one with the Link Index,
            // todo : the model can also be saved only when this structure is changed.
            
            runtimeWorldData.links.Add(linkData);
        }
    }

    public void StartCombat(int stageIndex, int linkIndex)
    {
        if (stageIndex == 0)
        {
            Debug.LogError("Stage Index is 0, set to 1");
            stageIndex = 1;
        }
        
        //Save World Data
        SaveCurrentWorldData();
        
        ChangeState(GameState.Combat);
        FieldSystem.onCombatEnter.Invoke(true);
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
        return runtimeWorldData.discoveredWorldTileSet.Contains(tilePos);
    }
    
    public void AddPioneeredWorldTile(Vector3Int tilePos)
    {
        if (!runtimeWorldData.discoveredWorldTileSet.Add(tilePos));
    }
    
    private new void Awake()
    {
        base.Awake();
        if(this == null) return;

        runtimeWorldData = Instantiate(_defaultWorldData);
        runtimeWorldData.discoveredWorldTileSet = new HashSet<Vector3Int>();
        
        
        var watch = DGS.Stopwatch.StartNew();
        var qi = new QuestParser();
        Quests = qi.GetQuests();
        watch.Stop();
        Debug.Log($"<color=blue>Quest parse time: {watch.ElapsedMilliseconds}</color>");
    }

    private void Start()
    {
        #region ����Ʈ ����. ���߿� �ű� ����
        var watch = DGS.Stopwatch.StartNew();
        foreach (var quest in Quests)
        {
            // ���� ���۽� �����ϴ� ����Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GAME_START))
                OnGameStarted.AddListener(quest.OnConditionEventOccured);

            // ����Ʈ ���۽� Invoke �Լ� ȣ��
            quest.OnQuestStarted.AddListener(InvokeQuestStart);
            // ����Ʈ �Ϸ�� Invoke �Լ� ȣ��
            quest.OnQuestEnded.AddListener(InvokeQuestEnd);

            // ����Ʈ �Ϸ�� �����ϴ� ����Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedConditionEvent(q.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedGoalEvent(q.Index));

            // ����Ʈ ����, �Ϸ��� MOVE_TO ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedConditionEvent(pos));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedGoalEvent(pos));

            // ����Ʈ ����, �Ϸ���� KILL_LINK ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountConditionEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountGoalEvented);
             
            // ����Ʈ ����, �Ϸ���� KILL_UNIT ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u)=>quest.OnCountConditionEvented(u.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u)=>quest.OnCountGoalEvented(u.Index));

            // ����Ʈ ����, �Ϸ���� GET_ITEM, USE_TIEM ȣ��, ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener((i) => quest.OnCountGoalEvented(i.id)) ;

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener((i) => quest.OnCountGoalEvented(i.id)) ;
            
            // �÷��̾� �þ� ���� Ÿ��, ��ũ ���Խ��� �̺�Ʈ ����
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedConditionEvent(tile.hexPosition));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedGoalEvent(tile.hexPosition));
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedConditionEvent(link.linkIndex));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedGoalEvent(link.linkIndex));
            
            if (quest.ExpireTurn != -1)
                PlayerEvents.OnProcessedWorldTurn.AddListener((u) => { quest.ProgressExpireTurn();});
        }
        watch.Stop();
        Debug.Log($"<color=blue>Quest link time: {watch.ElapsedMilliseconds}</color>");

        OnNotifiedQuestStart.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, true); });
        OnNotifiedQuestEnd.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, false); });
        #endregion

        OnGameStarted?.Invoke();
        UIManager.instance.gameSystemUI.conversationUI.StartNextConversation();    //load previous quest when start game _ fix later
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
