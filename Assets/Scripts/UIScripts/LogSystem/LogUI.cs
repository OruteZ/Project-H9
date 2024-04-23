using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogUI : UISystem
{
    [SerializeField]
    private RectTransform _logView; // on/off 시 끄고 켜는 창

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
    private readonly string LOCALIZATION_PATH = "LogLocalizationTable";
    private Dictionary<int, string> localization;

    private const int LIMIT_TEXT_LENGTH = 500;

    private void Awake()
    {
        FileRead.ParseLocalization(in LOCALIZATION_PATH, out localization);

        UIManager.instance.onLevelUp.AddListener(ChangedLevel);
        UIManager.instance.onGetExp.AddListener(ChangedExp);
        UIManager.instance.onTSceneChanged.AddListener(TChangeScene);
        UIManager.instance.onTakeDamaged.AddListener(TakeDamaged);
        UIManager.instance.onStartAction.AddListener(StartAction);
        UIManager.instance.onNonHited.AddListener(NonHited);
        PlayerEvents.OnProcessedWorldTurn.AddListener(ProcessedWorldTurn);
        UIManager.instance.onStartedCombatTurn.AddListener(StartedCombatTurn);
        //UIManager.instance.onPlayerStatChanged.AddListener(ChangedPlayerStat); // 모든 플레이어 스탯변화 추적하기 힘들어 일단 주석처리
        InstantiateText();
        _FixedTextedPanelHeight = 0;
    }


    public override void CloseUI()
    {
        base.CloseUI();
        _logView.gameObject.SetActive(false);
    }

    public override void OpenUI()
    {
        base.OpenUI();
        _logView.gameObject.SetActive(true);
    }

    public void ViewToggle()
    {
        if (_logView.gameObject.activeSelf) CloseUI();
        else OpenUI();
    }

    private void ChangedExp(int exp)
    {
        var message = localization[1].Replace("{exp}", exp.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }
    private void ChangedLevel(int level)
    {
        var message = localization[2].Replace("{level}", level.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }

    private void ProcessedWorldTurn(int turn)
    {
        var message = localization[3].Replace("{turn}", turn.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }

    private void StartedCombatTurn(Unit unit)
    {
        var message = localization[4].Replace("{unitName}", unit.unitName.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }

    private void TakeDamaged(Unit unit, int damage, eDamageType.Type type)
    {
        var message = localization[5].Replace("{unitName}", unit.unitName.ToString());
        message = message.Replace("{damage}", damage.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }

    private void NonHited(Unit unit)
    {
        var message = localization[6].Replace("{unitName}", unit.unitName.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }
    
    private void TChangeScene(GameState gameState)
    {
        if (gameState == GameState.Combat)
        {
            var message = localization[7];
            _builder.Append($"{message}\n");
            UpdateText();
        }
    }

    private void StartAction(Unit unit, BaseAction action)
    {
        string actionType = localization[11];
        switch (action.GetActionType())
        {
            case ActionType.Attack: actionType = localization[12]; break;
            case ActionType.Dynamite: actionType = localization[13]; break;
            case ActionType.Fanning: actionType = localization[14]; break;
            case ActionType.Hemostasis: actionType = localization[15]; break;
            case ActionType.Move: actionType = localization[16]; break;
            case ActionType.Reload: actionType = localization[17]; break;
            case ActionType.ItemUsing: 
                int itemIdx = ((ItemUsingAction)action).GetItem().GetData().nameIdx;
                ItemScript script = GameManager.instance.itemDatabase.GetItemScript(itemIdx);
                string itemName = script.GetName();
                var itemMessage = localization[18].Replace("{itemName}", itemName.ToString());
                actionType = itemMessage;
                break;
            default: break;
        }
        var message = localization[10].Replace("{unitName}", unit.unitName.ToString());
        message = message.Replace("{action}", actionType.ToString());
        _builder.Append($"{message}\n");
        UpdateText();
    }

    private void InstantiateText()
    {
        var target = Instantiate(_defaultTextPrefab);
        target.transform.SetParent(_logPanel);
        target.transform.localScale = Vector3.one; // 간혹 canvas의 scale이 변하는 일 때문에 설정해둠. scale 변환시킬 일 있으면 그 때 보자.
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
