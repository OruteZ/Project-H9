using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterStatTextElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    private Image _statIcon;
    private TextMeshProUGUI _statText;
    private string _statName;

    private bool _isSetContents = false;
    private bool _isOpenTooltip = false;
    public bool isMouseOver { get; private set; }
    private float _mouseOverCount = 0.0f;

    private bool _isTooltipOpenable = true;
    private string[] _tooltipExceptionStrings = { "Level", "Exp", "Name" };
    // Start is called before the first frame update
    void Awake()
    {
        isMouseOver = false;
        _statIcon = transform.GetChild(0).GetComponent<Image>();
        _statText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isTooltipOpenable) return;
        if (isMouseOver && !_isOpenTooltip && _isSetContents) 
        {
            _mouseOverCount += Time.deltaTime;
            if (_mouseOverCount > 0.5f) 
            {
                _isOpenTooltip = true;
                UIManager.instance.characterUI.characterStatUI.OpenCharacterTooltip(this, _statName, GetComponent<RectTransform>().position);
            }
        }
    }
    public void SetCharacterStatText(CharacterStatUIInfo info)
    {
        if (_statIcon is null) return;
        _statIcon.sprite = UIManager.instance.iconDB.GetIconInfo(info.statName + " Stat");
        _statName = info.statName;
        _statText.text = info.GetFinalStatValueString();
        _isSetContents = (_statName != "");

        foreach (string compStr in _tooltipExceptionStrings) 
        {
            if (compStr == info.statName) 
            {
                _isTooltipOpenable = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _statText.color = UICustomColor.TextHighlightColor;

        _isOpenTooltip = false;
        isMouseOver = true;
        _mouseOverCount = 0.0f;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _statText.color = Color.white;

        _isOpenTooltip = false;
        isMouseOver = false;
        _mouseOverCount = 0.0f;

        UIManager.instance.characterUI.characterStatUI._characterStatTooltip.GetComponent<CharacterTooltip>().CloseUI();
    }
}
