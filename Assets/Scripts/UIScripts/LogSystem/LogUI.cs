using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogUI : UISystem
{
    [SerializeField]
    private RectTransform _logPanel; // text 들을 담을 수 있으며, text 들의 합만큼의 크기를 지닐 것임.
    private float _FixedTextedPanelHeight; // "length text 0 ~ n-1" + length text n

    [SerializeField]
    private GameObject _defaultTextPrefab;

    [SerializeField]
    private ScrollRect _scroll;

    private List<TMP_Text> _textCaches = new List<TMP_Text>();
    private List<ContentSizeFitter> _sizeFilterCaches = new List<ContentSizeFitter>();
    private StringBuilder _builder = new StringBuilder();
    private int _curTextlistIndex = -1;
    private float _beforeHeight = 0;

    private readonly int LIMIT_TEXT_LENGTH = 500;

    private void Awake()
    {
        UIManager.instance.onLevelUp.AddListener(ChangedLevel);
        UIManager.instance.onGetExp.AddListener(ChangedExp);
        UIManager.instance.onTSceneChanged.AddListener(TChangeScene);
        //UIManager.instance.onActionChanged.AddListener(ChangedAction); // 관련된 Action이 너무 많아 분리하기위해 일단 주석처리
        UIManager.instance.onTakeDamaged.AddListener(TakeDamaged);
        UIManager.instance.onStartAction.AddListener(StartAction);
        UIManager.instance.onNonHited.AddListener(NonHited);
        //UIManager.instance.onPlayerStatChanged.AddListener(ChangedPlayerStat); // 모든 플레이어 스탯변화 추적하기 힘들어 일단 주석처리
        InstantiateText();
        _FixedTextedPanelHeight = 0;
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
    }

    private void ChangedExp(int exp)
    {
        _builder.Append($"{exp} 경험치를 얻었습니다..\n");
        UpdateText();
    }
    private void ChangedLevel(int level)
    {
        _builder.Append($"레벨업! {level} 레벨이 되었습니다.\n");
        UpdateText();
    }

    private void StartedTurn(Unit unit)
    {
        _builder.Append($"{unit.unitName}의 {unit.currentRound}번째 차례\n");
        UpdateText();
    }

    private void TakeDamaged(Unit unit, int damage, eDamageType.Type type)
    {
        _builder.Append($"{unit.unitName}에게 {damage} 피해.\n");
        UpdateText();
    }

    private void NonHited(Unit unit)
    {
        _builder.Append($"{unit.unitName}은 회피했다.\n");
        UpdateText();
    }
    
    private void TChangeScene(GameState gameState)
    {
        if (gameState == GameState.Combat)
        {
            _builder.Append("- 전투 시작 -\n");
            UpdateText();
        }
    }

    private void StartAction(Unit unit, BaseAction action)
    {
        string actionType = "UnknownAction";
        switch (action.GetActionType())
        {
            case ActionType.Attack: actionType = "공격"; break;
            case ActionType.Dynamite: actionType = "다이너마이트"; break;
            case ActionType.Fanning: actionType = "패닝"; break;
            case ActionType.Hemostasis: actionType = "지혈"; break;
            case ActionType.Move: actionType = "이동"; break;
            case ActionType.Reload: actionType = "재장전"; break;
            case ActionType.ItemUsing: 
                int itemIdx = ((ItemUsingAction)action).GetItem().GetData().nameIdx;
                ItemScript script = GameManager.instance.itemDatabase.GetItemScript(itemIdx);
                string itemName = script.GetName();
                actionType = $"{itemName} 사용";
                break;
            default: break;
        }
        _builder.Append($"{unit.unitName}의 {actionType}.\n");
        UpdateText();
    }

    private void InstantiateText()
    {
        var target = Instantiate(_defaultTextPrefab);
        target.transform.SetParent(_logPanel);
        _textCaches.Add(target.GetComponent<TMP_Text>());
        _sizeFilterCaches.Add(target.GetComponent<ContentSizeFitter>());
        _curTextlistIndex++;
    }

    private void UpdateText()
    {
        _textCaches[_curTextlistIndex].SetText(_builder);
        _sizeFilterCaches[_curTextlistIndex].SetLayoutVertical();
        float newHeight = _beforeHeight + _textCaches[_curTextlistIndex].rectTransform.sizeDelta.y;
        _logPanel.sizeDelta = new Vector2(_logPanel.sizeDelta.x, newHeight);
        _scroll.verticalScrollbar.value = 0; // 스크롤이 가장 아래로 내려오도록 조정

        // 크기가 일정 수준 이상이라면, 다음에 사용할 텍스트 오브젝트를 미리 만든다.
        if (LIMIT_TEXT_LENGTH < _textCaches[_curTextlistIndex].text.Length)
        {
            _builder.Clear(); // 이미 쓰여진 텍스트 오브젝트는 교체하지 않을 예정이므로, stringBuilder를 비운다.
            _beforeHeight = _textCaches[_curTextlistIndex].rectTransform.sizeDelta.y;
            _FixedTextedPanelHeight = _FixedTextedPanelHeight + _textCaches[_curTextlistIndex].rectTransform.sizeDelta.y;
            InstantiateText();
        }
        _logPanel.sizeDelta = new Vector2(_logPanel.sizeDelta.x, _FixedTextedPanelHeight + _textCaches[_curTextlistIndex].rectTransform.sizeDelta.y);
    }
}
