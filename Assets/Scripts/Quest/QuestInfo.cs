using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 1. CSV Init : 모든 퀘스트 정보 읽어오기
/// 2. Localization : Tooltip, Name 등 국가별 언어 읽어오기
/// 3. Async : 현재 유저 데이터에 존재하는, 퀘스트 진행 상황 동기화
/// 4. GetEvent : 사용 가능한 퀘스트를 Start에 연결
/// 5. GoalEvent : 현재 진행중인 퀘스트를 Event에 연결
/// - GoalEvent에서 완료 조건이 된다면 EndQuest로 종료
/// </summary>
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
                                , KILL_TARGET = 1 << 7 }; // 퀘스트의 연결을 Bit마스크로 확인용
    public UnityEvent<int> OnQuestEnded = new UnityEvent<int>();

    private int _index;
    private int _questType;
    private string _questName;
    private string _questTooltip;
    private int _startScript;
    private int _endScript;

    // conditions
    private QUEST_EVENT _conditionBit;
    private int[] _conditionArguments;
    private int _expireTurn;
    private QUEST_EVENT _goalBit;
    private int[] _goalArguments;

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

    public QuestInfo(int index
                    , int questType
                    , string questName
                    , string questTooltip
                    , int startScript
                    , int endScript
                    , QUEST_EVENT conditionBit
                    , int[] conditionArgument
                    , int expireTurn
                    , QUEST_EVENT goalBit
                    , int[] goalArguments
                    , int moneyReward
                    , int expReward
                    , int itemReward
                    , int skillReward)
    {
        _index = index;
        _questType = questType;
        _questName = questName;
        _questTooltip = questTooltip;
        _startScript = startScript;
        _endScript = endScript;
        _conditionBit = conditionBit;
        _conditionArguments = conditionArgument;
        _expireTurn = expireTurn;
        _goalBit = goalBit;
        _goalArguments = goalArguments;
        _moneyReward = moneyReward;
        _expReward = expReward;
        _itemReward = itemReward;
        _skillReward = skillReward;

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

    // 게임 저장, 로드 시 현재 상황을 저장하기 위함.
    public void SetProgress(bool isInProgress, int[] curGoalArguments, bool isCleared)
    {
        _isInProgress = isInProgress;
        _curGoalArguments = curGoalArguments;
        _isCleared = isCleared;
    }

    // Game Start 처럼 무슨 일이 1회성으로 벌어진 이벤트.
    // Game start 외에 쓸 곳이 없는데, Game start 자체가 임시로 해둔 것이라 빼는 것도 고려 중.
    public void OnOccurConditionEvented()
    {
        if (!_isInProgress)
        {
            StartQuest();
        }
    }

    public void OnOccurQuestConditionEvented(int targetIndex)
    {
        if (!_isInProgress)
        {
            if (QuestEvent(ref _curConditionArguments, ref _conditionArguments, targetIndex))
                StartQuest();
        }
    }

    #region Add Event
    // 적 처치, 아이템 획득과 같은 KeyValue의 증가 이벤트
    // ex. [적의번호, 적을처치해야하는 수]
    public void OnAddConditionEvented(int targetIndex, int value)
    {
        if (!_isInProgress)
        {
            if (AddEvent(ref _curConditionArguments, ref _conditionArguments, targetIndex, value))
                StartQuest();
        }
    }

    public void OnAddGoalEvented(int targetIndex, int value)
    {
        if (_isInProgress)
        {
            if (AddEvent(ref _curGoalArguments, ref _goalArguments, targetIndex, value))
                EndQuest();
        }
    }

    #endregion

    #region Renew Event
    // 모든 인자가 같아야 하는 Event에 할당 
    public void OnRenewConditionEvented(int[] arguments)
    {
        if (!_isInProgress)
        {
            if (RenewEvent(ref _curConditionArguments, ref _conditionArguments, ref arguments))
                StartQuest();
        }
    }

    public void OnRenewGoalEvented(int[] arguments)
    {
        if (_isInProgress)
        {
            if (RenewEvent(ref _curGoalArguments, ref _goalArguments, ref arguments))
                EndQuest();
        }
    }
    #endregion

    // 플레이어 Move 인자는 더 추가 될 가능성이 있어서 별도로 빼 둠
    // 플레이어가 World에 있을 때만, 월드가 여러 개(지역별로) 나누어질 가능성이 있어서 해당 인덱스도.
    public void OnPlayerMovedConditionEvented(Unit player)
    {
        if (_isCleared) return;

        if (!GameManager.instance.CompareState(GameState.World))
            return;
        
        if (!_isInProgress)
        {
            if (OnPlayerEvent(ref _curConditionArguments, ref _conditionArguments, player.hexPosition))
                StartQuest();
        }
    }

    public void OnPlayerMovedGoalEvented(Unit player)
    {
        if (_isCleared) return;

        if (!GameManager.instance.CompareState(GameState.World))
            return;

        if (_isInProgress)
        {
            if (OnPlayerEvent(ref _curGoalArguments, ref _goalArguments, player.hexPosition))
                EndQuest();
        }
    }

    /// ===========================================================================
    private void StartQuest()
    {
        if (_isCleared) return;

        _isInProgress = true;
        Debug.Log($"[{_index}]'{_questName}' 퀘스트 시작, startScript 시작, UI연동 해야됨");
    }

    private void EndQuest() // 무조건 보상을 받는 케이스만 존재.
    {
        _isInProgress = false;
        _isCleared = true;
        Debug.Log($"[{_index}]'{_questName}'퀘스트 완료, 보상 받는 코드, endScript 호출, UI연동 해야됨");
        OnQuestEnded?.Invoke(_index);
    }


    private bool QuestEvent(ref int[] curArgument, ref int[] goalArgument, int value)
    {
        if (goalArgument[0] != value)
            return false;
        curArgument[0] = value;
        return true;
    }

    private bool AddEvent(ref int[] curArgument, ref int[] goalArgument, int goalType, int value)
    {
        if (curArgument[0] != goalType)
            return false;

        curArgument[1] += value;
        if (curArgument[1] < goalArgument[1])
            return false;

        curArgument[1] = goalArgument[1]; // UI에서 표시할 때, 범위가 넘어가지 않도록 함.
        return true;
    }

    // ownArgument: 직전까지의 플레이어의 위치(값)
    // goalArrgument: 플레이어가 희망하는 위치
    // curArgument: 플레이어가 지금 도달한 위치
    private bool RenewEvent(ref int[] ownArgument, ref int[] goalArgument, ref int[] curArgument)
    {
        bool isSame = true;
        for (int i = 0; i < goalArgument.Length; i++)
        {
            ownArgument[i] = curArgument[i];
            if (goalArgument[i] != curArgument[i])
                isSame = false;
        }

        if (!isSame)
            return false;

        return true;
    }

    private bool OnPlayerEvent(ref int[] cur, ref int[] goal, Vector3Int pos)
    {
        cur[0] = pos.x;
        cur[1] = pos.y;
        cur[2] = pos.z;

        for (int i = 0; i < goal.Length; i++)
        {
            if (cur[i] != goal[i])
                return false;
        }

        return true;
    }
}