using System.Collections.Generic;
using UnityEngine;

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
    private int _index;
    private int _questType;
    private string _questName;
    private string _questTooltip;
    private int _startScript;
    private int _endScript;

    // conditions
    private string _getCondition;
    private int[] _getConditionArguments;
    private int _expireTurn;
    private int _goalType;
    private int[] _goalArguments;
    
    // rewards
    private int _moneyReward;
    private int _expReward;
    private int _itemReward;
    private int _skillReward;

    // current infos (not table)
    private bool _isInProgress = false;
    private int _curTurn;
    private int[] _curGoalArguments;

    public QuestInfo(int index
                    , int questType
                    , string questName
                    , string questTooltip
                    , int startScript
                    , int endScript
                    , string getCondition
                    , int expireTurn
                    , int goalType
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
        _getCondition = getCondition;
        _expireTurn = expireTurn;
        _goalType = goalType;
        _goalArguments = goalArguments;
        _moneyReward = moneyReward;
        _expReward = expReward;
        _itemReward = itemReward;
        _skillReward = skillReward;
    }

    // 게임 저장, 로드 시 현재 상황을 저장하기 위함.
    public void SetProgress(bool isInProgress, int[] curGoalArguments)
    {
        _isInProgress = isInProgress;
        _curGoalArguments = curGoalArguments;
    }

    // 적 처치, 아이템 획득과 같은 인자가 1개씩 증가하는 Event에 할당
    public void OnAddEvented(int i)
    {
        _curGoalArguments[0] += i;
        if (_curGoalArguments[0] < _goalArguments[0])
            return;

        _curGoalArguments[0] = _goalArguments[0]; // UI에서 표시할 때, 범위가 넘어가지 않도록 함.
        EndQuest();
    }

    // 좌표 도착처럼 모든 인자가 같아야 하는 Event에 할당
    public void OnRenewEvented(int[] arguments)
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            if (_goalArguments[i] != arguments[i])
                return;
        }

        _curGoalArguments = arguments;
        EndQuest();
    }

    public void StartQuest()
    {
        _isInProgress = true;
        Debug.Log("퀘스트 시작, startScript 시작");
    }

    public void EndQuest() // 무조건 보상을 받는 케이스만 존재.
    {
        _isInProgress = false;
        Debug.Log("퀘스트 완료, 보상 받는 코드, endScript 호출");
    }
}
