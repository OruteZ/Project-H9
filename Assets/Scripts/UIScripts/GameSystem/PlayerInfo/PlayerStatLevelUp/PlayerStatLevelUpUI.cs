using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatLevelInfo 
{
    public string statName { get; private set; }
    public int statIncreaseValue { get; private set; }
    public int statLevel { get; private set; }

    public PlayerStatLevelInfo(string name, int increse) 
    {
        statName = name;
        statIncreaseValue = increse;
        statLevel = 0;
    }

    public void LevelUpStat() 
    {
        if(IsLevelUpFully()) return;
        statLevel++;
        var stat = GameManager.instance.playerStat;
        if (statName == "Concentration")
        {
            stat.SetOriginalStat(StatType.Concentration, 
                stat.GetOriginalStat(StatType.Concentration) + statIncreaseValue);
        }
        else if (statName == "Sight Range")
        {
            stat.SetOriginalStat(StatType.SightRange, 
                stat.GetOriginalStat(StatType.SightRange) + statIncreaseValue);
            FieldSystem.unitSystem.GetPlayer().ReloadSight();
        }
        else if (statName == "Speed")
        {
            stat.SetOriginalStat(StatType.Speed, 
                stat.GetOriginalStat(StatType.Speed) + statIncreaseValue);
        }
        UIManager.instance.onPlayerStatChanged.Invoke();
    }
    public bool IsLevelUpFully() 
    {
        return (statLevel >= 6);
    }
    public int GetPlayerCurrentValue()
    {
        UnitStat playerStat = GameManager.instance.playerStat;
        if (statName == "Concentration")
        {
            return playerStat.concentration;
        }
        else if (statName == "Sight Range")
        {
            return playerStat.sightRange;
        }
        else if (statName == "Speed")
        {
            return playerStat.speed;
        }

        Debug.LogError("유효하지 않은 statName입니다.");
        return -1;
    }
}

public class PlayerStatLevelUpUI : UISystem
{
    [SerializeField] private GameObject _statLevelUpButton;
    [SerializeField] private GameObject _statLevelUpButtonText;
    [SerializeField] private GameObject _statLevelUpWindow;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _statLevelUpTitleText;
    [SerializeField] private GameObject[] _statCards;
    [SerializeField] private GameObject _statSelectButton;
    [SerializeField] private GameObject _statTooltip;

    private int _sp = 0;
    private int _statPoint { 
        get 
        { 
            return _sp; 
        } 
        set 
        {
            _sp = value;
            if (_sp <= 0)
            {
                _statLevelUpButtonText.GetComponent<TextMeshProUGUI>().text = "";
                UIManager.instance.gameSystemUI.alarmUI.DeleteAlarmUI(AlarmType.StatPoint);
            }
            else
            {
                _statLevelUpButtonText.GetComponent<TextMeshProUGUI>().text = value.ToString();
                UIManager.instance.gameSystemUI.alarmUI.AddAlarmUI(AlarmType.StatPoint);
            }
        } 
    }
    private bool _isOpenUI = false;
    public PlayerStatLevelInfo[] statLevels { get; private set; }
    public int selectedCardNumber { get; private set; }
    public bool isSelectedSomeCard { get; private set; }


    private int _appearSpeed = 5;
    private void Awake()
    {
        _statLevelUpButton.SetActive(false);

        ClosePlayerStatLevelUpUI();
        //_statLevelUpWindow.SetActive(false);
    }
    void Start()
    {
        statLevels = new PlayerStatLevelInfo[3];
        statLevels[0] = new PlayerStatLevelInfo("Concentration", 10);
        statLevels[1] = new PlayerStatLevelInfo("Sight Range", 1);
        statLevels[2] = new PlayerStatLevelInfo("Speed", 10);

        selectedCardNumber = -1;
        isSelectedSomeCard = false;
        ClosePlayerStatLevelUpUI();
        _statTooltip.SetActive(false);

        UIManager.instance.onTSceneChanged.AddListener((gs) =>
            {
                if (gs == GameState.Combat)
                {
                    _statLevelUpButton.SetActive(false);
                }
                else if(_statPoint > 0)
                {
                    _statLevelUpButton.SetActive(true);
                }
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (!_statLevelUpButton.activeSelf) return;
        if (Input.GetKeyDown(HotKey.openStatLevelUpUIKey)) 
        {
            OpenPlayerStatLevelUpUI();
        }
        float[] appearTargetValue = { 380, 0, 192 / 255.0f };
        float[] disappearTargetValue = 
        {
            Camera.main.pixelHeight / 2  + _statLevelUpTitleText.GetComponent<RectTransform>().sizeDelta.y / 2, 
            -(Camera.main.pixelHeight / 2 + _statCards[0].GetComponent<RectTransform>().sizeDelta.y / 2),
            0
        };
        Vector3 pos;
        Color color;
        float alpha;
        float[] targetValue;
        if (_isOpenUI)
        {
            targetValue = appearTargetValue;
        }
        else
        {
            targetValue = disappearTargetValue;
        }
        pos = _statLevelUpTitleText.GetComponent<RectTransform>().localPosition;
        LerpCalculation.CalculateLerpValue(ref pos.y, targetValue[0], _appearSpeed);
        _statLevelUpTitleText.GetComponent<RectTransform>().localPosition = pos;
        if (pos.y == disappearTargetValue[0]) _statLevelUpTitleText.SetActive(false);

            for (int i = 0; i < _statCards.Length; i++)
        {
            pos = _statCards[i].GetComponent<RectTransform>().localPosition;
            if (selectedCardNumber != i || _isOpenUI)
            {
                LerpCalculation.CalculateLerpValue(ref pos.y, targetValue[1], _appearSpeed);
                _statCards[i].GetComponent<RectTransform>().localPosition = pos;
            }

            alpha = _statCards[i].GetComponent<CanvasGroup>().alpha;
            if (!_isOpenUI)
            {
                LerpCalculation.CalculateLerpValue(ref alpha, 0, _appearSpeed);
                _statCards[i].GetComponent<CanvasGroup>().alpha = alpha;
            }

            if (pos.y == disappearTargetValue[1] || alpha == 0) 
            {
                _statCards[i].SetActive(false);
            }
        }

        color = _background.GetComponent<Image>().color;
        LerpCalculation.CalculateLerpValue(ref color.a, targetValue[2], _appearSpeed);
        _background.GetComponent<Image>().color = color;
        if (color.a == disappearTargetValue[2]) _background.SetActive(false);
    }
    public int GetPlayerStatPoint() 
    {
        return _statPoint;
    }
    public void AddPlayerStatPoint() 
    {
        _statPoint++;
        _statLevelUpButton.SetActive(GameManager.instance.CompareState(GameState.World));
        PlayerEvents.OnIncStatPoint?.Invoke();
    }

    public void OpenPlayerStatLevelUpUI()
    {
        if (!GameManager.instance.CompareState(GameState.World)) return;
        if (_isOpenUI) return;
        int cnt = 0;
        for (int i = 0; i < _statCards.Length; i++)
        {
            if (statLevels[i].IsLevelUpFully())
            {
                cnt++;
            }
        }
        if (cnt >= _statCards.Length) return;

        OpenUI();
        _statLevelUpWindow.transform.SetParent(UIManager.instance.HotCanvas.transform);
        UIManager.instance.SetLogCanvasState(false);
        _isOpenUI = true;
        selectedCardNumber = -1;
        isSelectedSomeCard = false;

        //Set Title Position
        _statLevelUpTitleText.SetActive(true);
        Vector3 pos = _statLevelUpTitleText.GetComponent<RectTransform>().localPosition;
        pos.y = Camera.main.pixelHeight / 2  + _statLevelUpTitleText.GetComponent<RectTransform>().sizeDelta.y / 2;
        _statLevelUpTitleText.GetComponent<RectTransform>().localPosition = pos;

        //Set Card Position
        for (int i = 0; i < _statCards.Length; i++) 
        {
            pos = _statCards[i].GetComponent<RectTransform>().localPosition;
            pos.y = -(Camera.main.pixelHeight / 2 + _statCards[i].GetComponent<RectTransform>().sizeDelta.y / 2 * (i + 1));
            _statCards[i].GetComponent<RectTransform>().localPosition = pos;
            _statCards[i].GetComponent<PlayerStatLevelUpElement>().SetPlayerStatLevelUpCard(statLevels[i]);
            _statCards[i].GetComponent<CanvasGroup>().alpha = 1;
        }
        _statSelectButton.GetComponent<PlayerStatLevelUpSelectButton>().InitPlayerStatLevelUpSelectButton();

        //Set Background Color
        _background.SetActive(true);
        Color color = _background.GetComponent<Image>().color;
        color.a = 0;
        _background.GetComponent<Image>().color = color;

        _statSelectButton.SetActive(false);
        _statLevelUpWindow.SetActive(true);
    }
    private void ClosePlayerStatLevelUpUI() 
    {
        _isOpenUI = false;
        for (int i = 0; i < _statCards.Length; i++)
        {
            _statCards[i].GetComponent<PlayerStatLevelUpElement>().CloseUI();
        }
        _statSelectButton.SetActive(false);
        _statLevelUpWindow.SetActive(false);

        UIManager.instance.SetLogCanvasState(true);
        CloseUI();
    }

    public void ClickStatCard(int cardNumber, bool isSelected) 
    {
        //if (isSelectedSomeCard && selectedCardNumber != cardNumber) return;
        isSelectedSomeCard = isSelected;
        if (isSelected)
        {
            selectedCardNumber = cardNumber;
            for (int i = 0; i < _statCards.Length; i++)
            {
                if (i != selectedCardNumber)
                {
                    _statCards[i].GetComponent<PlayerStatLevelUpElement>().SetSelectedState(false);
                }
            }
        }
        else
        {
            selectedCardNumber = -1;
        }
        _statSelectButton.SetActive(isSelected);
    }
    public void ClickSelectButton() 
    {
        if (!isSelectedSomeCard) return;

        string levelUpStatName = _statCards[selectedCardNumber].GetComponent<PlayerStatLevelUpElement>().statLevelInfo.statName;
        for (int i = 0; i < statLevels.Length; i++) 
        {
            if (levelUpStatName == statLevels[i].statName)
            {
                statLevels[i].LevelUpStat();
            }
        }

        _statPoint--;
        if (_statPoint <= 0)
        {
            _statLevelUpButton.SetActive(false);
        }
        ClosePlayerStatLevelUpUI();
    }

    public void OpenPlayerStatLevelUpTooltip(PlayerStatLevelInfo info, Vector3 pos) 
    {
        _statTooltip.GetComponent<PlayerStatLevelUpTooltip>().SetStatLevelUpTooltip(info, pos);
        _statTooltip.SetActive(true);
    }
    public void ClosePlayerStatLevelUpTooltip()
    {
        _statTooltip.SetActive(false);
    }
}
