using System;
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

    [HideInInspector]
    public UserData user;
    public Inventory playerInventory;
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

    [Header("Player Info"), SerializeField]
    private UnitStat startPlayerStat; // 시작 시 플레이어 스탯을 정의

    [FormerlySerializedAs("_playerWeaponIndex")] public int playerWeaponIndex;
    public GameObject playerModel;
    public List<int> playerPassiveIndexList;
    public List<int> playerActiveIndexList;
    
    public UnityEvent<int> onPlayerCombatFinished = new UnityEvent<int>(); // <LinkIndex>, Combat manager ��ũ��Ʈ�� Player �׼� ��ũ��Ʈ(not player data)�� ������ �ű�����

    public OptionSetting initialOptionSetting;
    
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

    public CLevelSystem LevelSystem;

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

        playerPassiveIndexList = playerPassiveIndexList.Distinct().ToList();
        playerActiveIndexList = playerActiveIndexList.Distinct().ToList();
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

            if (user.optionSetting.lauguage == ScriptLanguage.NULL) 
            {
                user.optionSetting = initialOptionSetting;
            }
            UserAccount.Language = user.optionSetting.lauguage;
            UIManager.instance.scriptLanguage = user.optionSetting.lauguage;

            LevelSystem = new CLevelSystem(user.Level, user.EXP);
            if (user.Stat == null)
            {
                user.Stat = (UnitStat)startPlayerStat.Clone();
            }
            runtimeWorldData.playerPosition = user.Position;

            // 적, 인벤토리, 현재 장착 무기, 스킬 등 로드 
            GameManager.instance.backToWorldTrigger = true; // Turn 진행 중 저장됐을 것을 생각해서 ...
        }

        if (user.isFirstOpen)
        {
            GameManager.instance.backToWorldTrigger = false;
            user.isFirstOpen = false;
        }

        playerInventory = new Inventory();
        playerInventory.AddGold(user.money);
        if (user.equippedItemIndex != 0)
        {
            playerWeaponIndex = user.equippedItemIndex;
        }
        playerInventory.InitEquippedItem(Item.CreateItem(itemDatabase.GetItemData(playerWeaponIndex)));

        for (int i = 0; i < user.inventory.Count; i++)
        {
            Item item = null;
            if (user.inventory[i].index != 0)
            {
                item = Item.CreateItem(itemDatabase.GetItemData(user.inventory[i].index));
                item.SetStackCount(user.inventory[i].stack);
                playerInventory.TryAddItem(item);
            }
        }

        var watch = DGS.Stopwatch.StartNew();
        var qi = new QuestParser();
        Quests = qi.GetQuests();
        watch.Stop();
        Debug.Log($"<color=blue>Quest parse time: {watch.ElapsedMilliseconds}</color>");

    }

    private void Start()
    {
        QuestCallback questCallback = new QuestCallback(Quests
                                                        , user
                                                        , OnGameStarted
                                                        , OnNotifiedQuestEnd
                                                        , OnNotifiedQuestStart
                                                        , InvokeQuestStart
                                                        , InvokeQuestEnd);
        OnGameStarted?.Invoke();
        UIManager.instance.gameSystemUI.conversationUI.StartNextConversation();    //load previous quest when start game _ fix later
        InfoPopup.instance.InitCallback(user);
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

    public void Save(bool isAutoSave = false, bool isBrancheSave = false)
    {
        if (user == null) Debug.Log($"try saved, but user is null");
        user.Level = LevelSystem.Level;
        user.EXP = LevelSystem.CurExp;
        var player = FieldSystem.unitSystem.GetPlayer();
        user.Position = player.hexPosition;
        user.SaveTime = DateTime.Now;

        // 현재 진행중인 메인퀘스트 제목을 description으로 할당
        foreach (var quest in Quests)
        {
            if (quest.QuestType == 1 && quest.IsInProgress && !quest.IsCleared)
            {
                user.Description = $"{quest.QuestName}";
                break;
            }
        }

        user.ClearedQuests.Clear();
        user.QuestProgress.Clear();
        foreach (var quest in Quests)
        {
            if (quest.IsCleared)
            {
                user.ClearedQuests.Add(quest.Index);
            }
            else
            {
                user.QuestProgress.Add(quest.Index, quest.GetProgress());
            }
        }

        //skill
        user.skillPoint = SkillManager.instance.GetSkillPoint();
        user.learnedSkills.Clear();
        foreach (var s in SkillManager.instance.GetAllLearnedSkills()) 
        {
            user.learnedSkills.Add(s.skillInfo.index);
        }

        //item
        user.money = playerInventory.GetGold();
        user.equippedItemIndex = playerInventory.GetEquippedItem().GetData().id;
        user.inventory.Clear();
        foreach (var i in playerInventory.GetInventory()) 
        {
            ItemSaveWrapper sw = new ItemSaveWrapper { index = 0, stack = 0 };
            if (i is not null)
            {
                sw.index = i.GetData().id;
                sw.stack = i.GetStackCount();
            }
            user.inventory.Add(sw);
        }
        //option
        user.optionSetting = UIManager.instance.pauseMenuUI.optionUI.GetOptionSetting();

        UserDataFileSystem.Save(in user);

        if (isAutoSave)
            UserDataFileSystem.AutoSave(in user);
        else
            UserDataFileSystem.Save(in user, isBrancheSave);
    }
}
