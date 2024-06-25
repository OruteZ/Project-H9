using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI.Extensions;
using DGS = System.Diagnostics;

public enum GameState
{
    Combat,
    World,
    Editor,
    None
}
public class GameManager : Generic.Singleton<GameManager>
{
    private const string COMBAT_SCENE_NAME = "CombatScene";

    public UserData user;
    public Inventory playerInventory = new Inventory();
    [SerializeField]
    public ItemDatabase itemDatabase;
    public WeaponDatabase weaponDatabase;
    
    // map data
    public WorldData runtimeWorldData;
    [SerializeField] private WorldData _defaultWorldData;
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

        //Debug.Log("Added item to inventory");
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
            UIManager.instance.gameSystemUI.playerStatLevelUpUI.AddPlayerStatPoint();
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

    public bool backToWorldTrigger = false;

    private UnityEvent OnGameStarted = new UnityEvent();
    private UnityEvent<QuestInfo> OnNotifiedQuestEnd = new UnityEvent<QuestInfo>();
    private UnityEvent<QuestInfo> OnNotifiedQuestStart = new UnityEvent<QuestInfo>();

    private void SaveCurrentWorldData()
    {
        runtimeWorldData.worldTurn = FieldSystem.turnSystem.turnNumber;

        runtimeWorldData.playerPosition = FieldSystem.unitSystem.GetPlayer().hexPosition;
        
        //save links
        runtimeWorldData.links = new List<LinkObjectData>();
        
        foreach (Link link in FieldSystem.tileSystem.GetAllTileObjects().Where(obj => obj is Link))
        {
            LinkObjectData linkData = new LinkObjectData();
            linkData.pos = link.hexPosition;
            linkData.rotation = link.gameObject.transform.rotation.eulerAngles.y;
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

        SaveCurrentWorldData();  //Save World Data
        Save(); // Save User Data. World Data랑 하나로 합칠 가능성이 높음.

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
        ChangeState(GameState.World);

        backToWorldTrigger = true;
        LoadingManager.instance.LoadingScene(worldSceneName, () =>
        {
            onPlayerCombatFinished?.Invoke(GetLinkIndex());
        });
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
        Application.targetFrameRate = 90;

        runtimeWorldData = Instantiate(_defaultWorldData);
        runtimeWorldData.discoveredWorldTileSet = new HashSet<Vector3Int>();
        
        
        var watch = DGS.Stopwatch.StartNew();
        var qi = new QuestParser();
        Quests = qi.GetQuests();
        watch.Stop();
        Debug.Log($"<color=blue>Quest parse time: {watch.ElapsedMilliseconds}</color>");

#if UNITY_EDITOR
        // 유니티 에디터에서 GameManager를 첫 시작 시, Default player 데이터를 생성함.
        if (DataLoader.IsReady == false)
        {
            DataLoader.New();
        }
#endif
        if (DataLoader.IsReady)
        {
            user = DataLoader.Data;
            DataLoader.Clear();

            if (user.Stat == null)
                user.Stat = (UnitStat)playerStat.Clone();
            runtimeWorldData.playerPosition = user.Position;

            // 적, 인벤토리, 현재 장착 무기, 스킬 등 로드 

            GameManager.instance.backToWorldTrigger = true; // Turn 진행 중 저장됐을 것을 생각해서 ...
        }

        if (user.isFirstOpen)
        {
            GameManager.instance.backToWorldTrigger = false;
            user.isFirstOpen = false;
        }
    }

    private void Start()
    {
        #region ����Ʈ ����. ���߿� �ű� ����
        var watch = DGS.Stopwatch.StartNew();
        foreach (var quest in Quests)
        {
            // 유저 데이터로부터, 이미 클리어한/진행중인 퀘스트인지 확인하는 부분 추가해야 함.

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GAME_START))
                OnGameStarted.AddListener(quest.OnConditionEventOccured);

            quest.OnQuestStarted.AddListener(InvokeQuestStart);
            quest.OnQuestEnded.AddListener(InvokeQuestEnd);

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedConditionEvent(q.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedGoalEvent(q.Index));

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedConditionEvent(pos));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedGoalEvent(pos));

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountConditionEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                onPlayerCombatFinished.AddListener(quest.OnCountGoalEvented);

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u) => quest.OnCountConditionEvented(u.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u) => quest.OnCountGoalEvented(u.Index));

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener((i) => quest.OnCountGoalEvented(i.id));

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener((i) => quest.OnCountGoalEvented(i.id));

            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedConditionEvent(tile.hexPosition));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedGoalEvent(tile.hexPosition));
            if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedConditionEvent(link.linkIndex));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedGoalEvent(link.linkIndex));

            if (quest.ExpireTurn != -1)
                PlayerEvents.OnProcessedWorldTurn.AddListener((u) => { quest.ProgressExpireTurn(); });
        }
        watch.Stop();
        Debug.Log($"<color=blue>Quest link time: {watch.ElapsedMilliseconds}</color>");

        OnNotifiedQuestStart.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, true); });
        OnNotifiedQuestEnd.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, false); });
        #endregion

        OnGameStarted?.Invoke();
        UIManager.instance.gameSystemUI.conversationUI.StartNextConversation();    //load previous quest when start game _ fix later

        if (!user.Events.TryGetValue("INFO_POPUP_MESSAGE_DO_MOVE", out var value) || value == 0)
        {
            InfoPopup.instance.Show(InfoPopup.MESSAGE.DO_MOVE);
            user.Events.TryAdd("INFO_POPUP_MESSAGE_DO_MOVE", 1);
        }

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_MOVE_GAGE", out var value) || value == 0)
            {
                if (type == StatType.CurActionPoint && stat.GetStat(type) != stat.GetStat(StatType.MaxActionPoint))
                {
                    InfoPopup.instance.Show(InfoPopup.MESSAGE.IT_IS_MOVE_GAGE);
                    user.Events.TryAdd("INFO_POPUP_MOVE_GAGE", 1);
                }
            }
        });

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_TURN_END", out var value) || value == 0)
            {
                if (type == StatType.CurActionPoint && stat.GetStat(type) == 0)
                {
                    InfoPopup.instance.Show(InfoPopup.MESSAGE.IT_IS_TURN_END);
                    user.Events.TryAdd("INFO_POPUP_TURN_END", 1);
                }
            }
        });

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
       {
           if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_HP", out var value) || value == 0)
           {
               if (type == StatType.CurHp && stat.GetStat(type) != stat.GetStat(StatType.MaxHp))
               {
                   InfoPopup.instance.Show(InfoPopup.MESSAGE.COMBAT_HP);
                   user.Events.TryAdd("INFO_POPUP_COMBAT_HP", 1);
               }
           }
       });

        UIManager.instance.onTSceneChanged.AddListener((scene) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_TURN", out var value) || value == 0)
            {
                if (scene == GameState.Combat)
                {
                    InfoPopup.instance.Show(InfoPopup.MESSAGE.COMBAT_TURN);
                    user.Events.TryAdd("INFO_POPUP_COMBAT_TURN", 1);
                }
            }
        });

        UIManager.instance.onTSceneChanged.AddListener((scene) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_ACTION", out var value) || value == 0)
            {
                if (scene == GameState.Combat)
                {
                    InfoPopup.instance.Show(InfoPopup.MESSAGE.COMBAT_ACTION);
                    user.Events.TryAdd("INFO_POPUP_COMBAT_ACTION", 1);
                }
            }
        });


        PlayerEvents.OnIncSkillPoint.AddListener(() =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_INCREASED_SP", out var value) || value == 0)
            {
                InfoPopup.instance.Show(InfoPopup.MESSAGE.INCREASED_SP);
                user.Events.TryAdd("INFO_POPUP_INCREASED_SP", 1);
            }
        });

        PlayerEvents.OnIncStatPoint.AddListener(() =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_INCREASED_STAT", out var value) || value == 0)
            {
                InfoPopup.instance.Show(InfoPopup.MESSAGE.INCREASED_STAT);
                user.Events.TryAdd("INFO_POPUP_INCREASED_STAT", 1);
            }
        });
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

    public void Save()
    {
        if (user == null) Debug.Log($"try saved, but user is null");
        var player = FieldSystem.unitSystem.GetPlayer();
        user.Position = player.hexPosition;
        user.Stat = (UnitStat)player.stat.Clone();
        UserDataFileSystem.Save(in user);
    }
}
