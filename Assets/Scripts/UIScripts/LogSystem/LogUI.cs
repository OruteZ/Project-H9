using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogUI : UISystem
{
    [SerializeField]
    private RectTransform _logPanel;

    [SerializeField]
    private GameObject _defaultTextPrefab;

    [SerializeField]
    private ScrollRect _scroll;

    private List<TMP_Text> _textCaches= new List<TMP_Text>();
    private List<ContentSizeFitter> _sizeFilterCaches = new List<ContentSizeFitter>();
    private StringBuilder _builder = new StringBuilder();
    private int _curTextlistIndex = -1;
    private float _beforeHeight = 0;
    
    private readonly int LIMIT_TEXT_LENGTH = 500;

    private void Awake()
    {
        UIManager.instance.onPlayerStatChanged.AddListener(ChangedPlayerStat);
        UIManager.instance.onTurnChanged.AddListener(ChangedTurn);
        UIManager.instance.onActionChanged.AddListener(ChangedAction);
        InstantiateText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangedPlayerStat();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangedTurn();
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
    }

    private void ChangedPlayerStat()
    {
        _builder.Append("플레이어 스탯이 변화했습니다.\n");
        UpdateText();
    }

    private void ChangedTurn()
    {
        _builder.Append("턴이 변화했습니다.\n");
        UpdateText();
    }

    private void ChangedAction()
    {
        _builder.Append("액션이 변화했습니다.\n");
        UpdateText();
    }

    private void InstantiateText()
    {
        _builder.Append("<color:red>새로운 영역입니다.</color>\n");
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
            InstantiateText();
        }
    }
}
