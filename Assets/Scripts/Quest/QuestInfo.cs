using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestInfo
{
    public enum QUEST_EVENT {   NULL
                                , GAME_START = 1 << 0
                                , MOVE_TO   = 1 << 1
                                , QUEST_END = 1 << 2
                                , SCRIPT    = 1 << 3
                                , GET_ITEM  = 1 << 4
                                , USE_ITEM  = 1 << 5
                                , KILL_LINK = 1 << 6
                                , KILL_UNIT = 1 << 7 
                                , LINK_IN_SIGHT = 1 << 8 
                                , TILE_IN_SIGHT = 1 << 9 }; // 퀘스트의 연결을 Bit마스크로 확인용
    public UnityEvent<QuestInfo> OnQuestStarted = new UnityEvent<QuestInfo>();
    public UnityEvent<QuestInfo> OnQuestEnded = new UnityEvent<QuestInfo>(); // 퀘스트 성공여부와 관련없이 종료
    public UnityEvent OnChangedProgress = new UnityEvent();

    private int _index;
    private int _questType;
    private string _questName;
    private string _questTooltip;
    private int _startConversation;
    private int _endConversation;

    // conditions
    private QUEST_EVENT _conditionBit;
    private int[] _conditionArguments;
    private int _expireTurn;
    private QUEST_EVENT _goalBit;
    private int[] _goalArguments;

    private int[] _pinTile;
    private int[] _createLink;

    // rewards
    private int _moneyReward;
    private int _expReward;
    private int _itemReward;
    private int _skillReward;

    // current infos (not table)
    private bool _isInProgress = false;
    private bool _isCleared = false;
    private int _curTurn;
    private int[] _curConditionArguments;
    private int[] _curGoalArguments;

    public int Index { get => _index; }
    public bool IsInProgress => _isInProgress;
    public bool IsCleared => _isCleared;
    public int QuestType { get => _questType; }
    public string QuestName { get => _questName; }
    public string QuestTooltip { get => _questTooltip; }
    public int StartConversation { get => _startConversation; }
    public int EndConversation { get => _endConversation; }
    public int ExpireTurn { get => _expireTurn; }
    public QUEST_EVENT GOAL_TYPE { get => _goalBit; }
    public int[] GoalArg{ get => _goalArguments; }
    public int[] CurArg { get => _curGoalArguments; }
    public int[] Pin { get => _pinTile; }
    public int CurTurn { get => _curTurn; } // ExpireTurn에서 시작하여 0으로 향할 남은 턴. ex. {CurTurn}턴 남음!
    public int MoneyReward { get => _moneyReward; }
    public int ExpReward { get => _expReward; }
    public int ItemReward { get => _itemReward; }
    public int SKillReward { get => _skillReward; }

    public QuestInfo(int index
                    , int questType
                    , string questName
                    , string questTooltip
                    , int startConversation
                    , int endConversation
                    , QUEST_EVENT conditionBit
                    , int[] conditionArgument
                    , int expireTurn
                    , QUEST_EVENT goalBit
                    , int[] goalArguments
                    , int[] pinTile
                    , int[] createLink
                    , int moneyReward
                    , int expReward
                    , int itemReward
                    , int skillReward)
    {
        _index = index;
        _questType = questType;
        _questName = questName;
        _questTooltip = questTooltip;
        _startConversation = startConversation;
        _endConversation = endConversation;
        _conditionBit = conditionBit;
        _conditionArguments = conditionArgument;
        _expireTurn = expireTurn;
        _goalBit = goalBit;
        _goalArguments = goalArguments;
        _pinTile = pinTile;
        _createLink = createLink;
        _moneyReward = moneyReward;
        _expReward = expReward;
        _itemReward = itemReward;
        _skillReward = skillReward;

        _curTurn = _expireTurn;
        _curConditionArguments = new int[_conditionArguments.Length];
        _curGoalArguments = new int[_goalArguments.Length];
    }

    public bool HasConditionFlag(QUEST_EVENT target)
    {
        if (_conditionBit.HasFlag(target)) return true;
        return false;
    }
    public bool HasGoalFlag(QUEST_EVENT target)
    {
        if (_goalBit.HasFlag(target)) return true;
        return false;
    }

    // 용량을 위해 Save 데이터를 클리어/클리어 아닌 것으로 나눠서 저장하다보니
    // 이 함수가 별도로 나올 수 밖에 없는게 기분나쁘긴 한데, 일단 용량을 택함.
    public void SetClear()
    {
        _isCleared = true;
    }
    // 게임 저장, 로드 시 현재 상황을 저장하기 위함.
    public void SetProgress(QuestSaveWrapper saveWrapper)
    {
        _isInProgress = saveWrapper.IsInProgress;
        _curConditionArguments = saveWrapper.CurConditionArguments;
        _curGoalArguments = saveWrapper.CurGoalArguments;
    }

    public QuestSaveWrapper GetProgress()
    {
        var save = new QuestSaveWrapper();
        save.Index = Index;
        save.IsInProgress = _isInProgress;
        save.CurConditionArguments = _curConditionArguments;
        save.CurGoalArguments = _curGoalArguments;
        return save;
    }

    // Game Start 처럼 무슨 일이 1회성으로 벌어진 이벤트.
    // Game start 외에 쓸 곳이 없는데, Game start 자체가 임시로 해둔 것이라 빼는 것도 고려 중.
    public void OnConditionEventOccured()
    {
        if (!_isInProgress)
        {
            StartQuest();
        }
    }

    public void OnAccordedConditionEvent(int index)
    {
        if (!_isInProgress)
        {
            if (AccordEvent(ref _curConditionArguments, ref _conditionArguments, index))
                StartQuest();
        }
    }
    public void OnAccordedGoalEvent(int index)
    {
        if (_isInProgress)
        {
            if (AccordEvent(ref _curGoalArguments, ref _goalArguments, index))
                SuccessQuest();
        }
    }

    #region Count Event
    // 전투, 살해 처럼 이벤트성으로 일어나지만 그 수는 카운트하는 이벤트
    // ex. [아이템 인덱스, 사용해야하는 수]
    public void OnCountConditionEvented(int targetIndex)
    {
        if (!_isInProgress)
        {
            if (CountEvent(ref _curConditionArguments, ref _conditionArguments, targetIndex))
                StartQuest();
        }
    }

    public void OnCountGoalEvented(int targetIndex)
    {
        if (_isInProgress)
        {
            if (CountEvent(ref _curGoalArguments, ref _goalArguments, targetIndex))
                SuccessQuest();
        }
    }
    #endregion

    // 플레이어 Move 인자는 더 추가 될 가능성이 있어서 별도로 빼 둠
    // 플레이어가 World에 있을 때만, 월드가 여러 개(지역별로) 나누어질 가능성이 있어서 해당 인덱스도.
    public void OnPositionMovedConditionEvent(Vector3Int position)
    {
        if (_isCleared) return;

        if (!GameManager.instance.CompareState(GameState.World))
            return;
        
        if (!_isInProgress)
        {
            if (OnPositionEvent(ref _curConditionArguments, ref _conditionArguments, position))
                StartQuest();
        }
    }

    public void OnPositionMovedGoalEvent(Vector3Int position)
    {
        if (_isCleared) return;

        if (!GameManager.instance.CompareState(GameState.World))
            return;

        if (_isInProgress)
        {
            if (OnPositionEvent(ref _curGoalArguments, ref _goalArguments, position))
                SuccessQuest();
        }
    }
    
    public void ProgressExpireTurn()
    {
        _curTurn--;
        if (_curTurn == 0)
        {
            FailQuest();
        }
    }

    /// ===========================================================================
    private void StartQuest()
    {
        if (_isCleared) return;

        _isInProgress = true;
        OnQuestStarted.Invoke(this);     
        PlayerEvents.OnStartedQuest.Invoke(this);
        
        
        if (_createLink is { Length: 4 })
        {
            int linkIdx = _createLink[0];
            Vector3Int linkHex = new Vector3Int(_createLink[1], _createLink[2], _createLink[3]);
            if (linkIdx <= 0)
            {
                Debug.LogError("퀘스트 링크 생성 실패: 링크 인덱스가 0 이하입니다.");
                return;
            }
            
            if (GameManager.instance.CompareState(GameState.Combat))
            {
                GameManager.instance.runtimeWorldData.TryAddLink(linkHex, 0, linkIdx);
            }
            else FieldSystem.tileSystem.AddLink(linkHex, 0, linkIdx);
        }
    }

    private void SuccessQuest()
    {
        _isInProgress = false;
        _isCleared = true;

        var itemDB = GameManager.instance.itemDatabase;
        if (_moneyReward != 0)
            GameManager.instance.playerInventory.AddGold(_moneyReward);
        if (_itemReward != 0)
            if (GameManager.instance.playerInventory.TryAddItem(Item.CreateItem(itemDB.GetItemData(_itemReward))))
            {
                Debug.Log($"퀘스트 완료 아이템을 받을 수 없습니다.: item code '{_itemReward}'");
            }
        if (_skillReward != 0)
        {
            bool isAlreadyLearned = false;
            foreach (var skill in SkillManager.instance.GetAllLearnedSkills()) 
            {
                if (skill.skillInfo.index == _skillReward) 
                {
                    isAlreadyLearned = true;
                    break;
                }
            }
            if (!isAlreadyLearned)
            {
                SkillManager.instance.LearnSkill(_skillReward, true);
                UIManager.instance.gameSystemUI.alarmUI.AddAlarmUI(SkillManager.instance.GetSkill(_skillReward));
            }
            else 
            {
                SkillManager.instance.AddSkillPoint(1);
            }
        }
        if (_expReward != 0)
        {
            LevelSystem.GetExpImmediately(_expReward);
        }
        PlayerEvents.OnSuccessQuest.Invoke(this);
        OnQuestEnded?.Invoke(this);
    }

    private void FailQuest()
    {
        _isInProgress = false;
        _isCleared = true;
        PlayerEvents.OnFailedQuest.Invoke(this);
        OnQuestEnded?.Invoke(this);
    }

    private bool AccordEvent(ref int[] curArgument, ref int[] goalArgument, int value)
    {
        OnChangedProgress?.Invoke();
        if (goalArgument[0] != value)
            return false;
        curArgument[0] = value;
        return true;
    }

    // 플레이어가 도달한 위치 이벤트
    private bool OnPositionEvent(ref int[] cur, ref int[] goal, Vector3Int pos)
    {
        cur[0] = pos.x;
        cur[1] = pos.y;
        cur[2] = pos.z;

        OnChangedProgress?.Invoke();
        for (int i = 0; i < goal.Length; i++)
        {
            if (cur[i] != goal[i])
                return false;
        }

        return true;
    }

    private bool CountEvent(ref int[] curArgument, ref int[] goalArgument, int goalType)
    {
        if (goalArgument[0] != goalType)
            return false;

        OnChangedProgress?.Invoke();
        curArgument[1] += 1;
        if (curArgument[1] < goalArgument[1])
            return false;
        return true;
    }
}

[SerializeField]
public class QuestSaveWrapper
{
    public int Index;
    public bool IsInProgress;
    public int[] CurGoalArguments;
    public int[] CurConditionArguments;
}