using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerStatLevelUpElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image[] _levelImages;
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _statDescriptionText;
    [SerializeField] private Image _statIcon;

    [SerializeField] private int _cardNumber;
    [SerializeField] private Sprite[] _cardIcons;
    public PlayerStatLevelInfo statLevelInfo { get; private set; }

    private bool _isMouseOver = false;
    private bool _isSelected = false;
    private float _scaleCorrection = 1.0f;
    private float _scaleCorrectSpeed = 5;

    public override void CloseUI()
    {
        isOpenUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpenUI) return;
        bool isSelectedSomeCard = UIManager.instance.gameSystemUI.playerStatLevelUpUI.isSelectedSomeCard;
        _scaleCorrection = 1.0f;
        _scaleCorrectSpeed = 5;
        if (_isMouseOver)
        {
            _scaleCorrection = 1.1f;
            if (Input.GetMouseButton(0))
            {
                _scaleCorrection *= 0.95f;
                _scaleCorrectSpeed = 20;
            }
            if (Input.GetMouseButtonUp(0) && !statLevelInfo.IsLevelUpFully()) 
            {
                _isSelected = !_isSelected;
                UIManager.instance.gameSystemUI.playerStatLevelUpUI.ClickStatCard(_cardNumber, _isSelected);
                _scaleCorrectSpeed = 20;
            }
        }
        if (_isSelected)
        {
            _scaleCorrection = 1.1f;
        }

        //scale correction
        if ((isSelectedSomeCard && !_isSelected) || statLevelInfo.IsLevelUpFully())
        {
            _scaleCorrection = 1.0f;
        }
        Vector3 scale = GetComponent<RectTransform>().localScale;
        float threshold = 0.001f;
        if (Mathf.Abs(scale.x - _scaleCorrection) > threshold)
        {
            scale.x = Mathf.Lerp(scale.x, _scaleCorrection, Time.deltaTime * _scaleCorrectSpeed);
        }
        else
        {
            scale.x = _scaleCorrection;
        }
        scale.y = scale.x;
        GetComponent<RectTransform>().localScale = scale;
    }
    public void SetPlayerStatLevelUpCard(PlayerStatLevelInfo info)
    {
        OpenUI();
        statLevelInfo = info;
        for (int i = 0; i < _levelImages.Length; i++)
        {
            _levelImages[i].color = Color.yellow;
            if (i + 1 > statLevelInfo.statLevel)
            {
                _levelImages[i].color /= 4;
            }
        }

        _statNameText.GetComponent<TextMeshProUGUI>().text = statLevelInfo.statName;

        if (statLevelInfo.statName == "Concentration") 
        {
            _statIcon.sprite = _cardIcons[0];
        }
        else if (statLevelInfo.statName == "Sight Range")
        {
            _statIcon.sprite = _cardIcons[1];
        }
        else if (statLevelInfo.statName == "Speed")
        {
            _statIcon.sprite = _cardIcons[2];
        }

        int currentStat = statLevelInfo.GetPlayerCurrentValue();
        int increseStat = statLevelInfo.statIncreaseValue;
        string descriptionText = currentStat.ToString();
        if (!statLevelInfo.IsLevelUpFully())
        {
            descriptionText += " -> " + (currentStat + increseStat).ToString();
        }
        _statDescriptionText.GetComponent<TextMeshProUGUI>().text = descriptionText;

        _isMouseOver = false;
        _isSelected = false;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public void SetSelectedState(bool isSelected) 
    {
        _isSelected = isSelected;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        bool isSelectedSomeCard = UIManager.instance.gameSystemUI.playerStatLevelUpUI.isSelectedSomeCard;
        //if (isSelectedSomeCard && !_isSelected) return;
        _isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
    }
}
