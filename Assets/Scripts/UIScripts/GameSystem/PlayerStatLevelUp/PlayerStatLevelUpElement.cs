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

    public string statName { get; private set; }
    public int statLevel { get; private set; }
    public bool isLevelUpFully { get; private set; }

    private bool _isMouseOver = false;
    private bool _isSelected = false;
    private float _scaleCorrection = 1.0f;
    private float _scaleCorrectSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        
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
            if (Input.GetMouseButtonUp(0) && !isLevelUpFully) 
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
        if ((isSelectedSomeCard && !_isSelected) || isLevelUpFully)
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
    public void SetPlayerStatLevelUpCard(int level, string name)
    {
        OpenUI();
        statLevel = level;
        for (int i = 0; i < _levelImages.Length; i++)
        {
            _levelImages[i].color = Color.yellow;
            if (i + 1 > level)
            {
                _levelImages[i].color /= 4;
            }
        }
        isLevelUpFully = (statLevel >= _levelImages.Length);

        statName = name;
        _statNameText.GetComponent<TextMeshProUGUI>().text = name;
        UnitStat playerStat = FieldSystem.unitSystem.GetPlayer().GetStat();
        int currentStat = 0;
        int increseStat = 0;
        if (name == "Concentration") 
        {
            _statIcon.sprite = _cardIcons[0];
            currentStat = playerStat.concentration;
            increseStat = UIManager.instance.gameSystemUI.playerStatLevelUpUI.statIncreseValue[0];
        }
        else if (name == "Sight Range")
        {
            _statIcon.sprite = _cardIcons[1];
            currentStat = playerStat.sightRange;
            increseStat = UIManager.instance.gameSystemUI.playerStatLevelUpUI.statIncreseValue[1];
        }
        else if (name == "Speed")
        {
            _statIcon.sprite = _cardIcons[2];
            currentStat = playerStat.speed;
            increseStat = UIManager.instance.gameSystemUI.playerStatLevelUpUI.statIncreseValue[2];
        }
        string descriptionText = currentStat.ToString();
        if (!isLevelUpFully)
        {
            descriptionText += " -> " + (currentStat + increseStat).ToString();
        }
        _statDescriptionText.GetComponent<TextMeshProUGUI>().text = descriptionText;

        _isMouseOver = false;
        _isSelected = false;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public void ClearPlayerStatLevelUpUICard(int level)
    {
        isLevelUpFully = (level >= _levelImages.Length);
        CloseUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
    }
}
