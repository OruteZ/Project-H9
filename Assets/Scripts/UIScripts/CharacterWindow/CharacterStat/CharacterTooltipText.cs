using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterTooltipText : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _subTooltip;
    [SerializeField] private GameObject _subTooltipText;

    private Color _orangeColor = new Color32(237, 146, 0, 255);
    private Color _greenColor = new Color32(18, 219, 36, 255);
    private Color _redColor = new Color32(219, 36, 18, 255);

    private bool _isSetContents = false;
    private bool _isOpenTooltip = false;
    private bool isMouseOver = false;
    private float _mouseOverCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseOver && !_isOpenTooltip && _isSetContents)
        {
            _mouseOverCount += Time.deltaTime;
            if (_mouseOverCount > 0.5f)
            {
                _isOpenTooltip = true;
                _subTooltip.SetActive(true);
            }
        }
    }

    public void SetCharacterTooltipText(string statName, string statType, float value, float xPosition) 
    {
        OpenUI();
        string valueStr = "";
        if (statType == "CharacterStat") 
        {
            valueStr = value.ToString();
            GetComponent<TextMeshProUGUI>().color = _orangeColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = _orangeColor;
            string str = "캐릭터 보너스";
            if (statName == "Additional Hit Rate" || statName == "Critical Chance" || statName == "Critical Damage")
            {
                if (FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType() == WeaponType.Revolver)
                {
                    str += " (리볼버)";
                }
                else if (FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType() == WeaponType.Repeater)
                {
                    str += " (리피터)";
                }
                else if (FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType() == WeaponType.Shotgun)
                {
                    str += " (샷건)";
                }
            }
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = str;
        }
        else if (statType == "WeaponStat")
        {
            valueStr = value.ToString();
            GetComponent<TextMeshProUGUI>().color = _greenColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = _greenColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = "장착 무기 보너스";
        }
        else if (statType == "SkillStat")
        {
            valueStr = value.ToString();
            GetComponent<TextMeshProUGUI>().color = _redColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = _redColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = "스킬 보너스";
        }
        else
        {
            valueStr = "+";
            GetComponent<TextMeshProUGUI>().color = Color.white;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = "";
        }
        _isSetContents = (valueStr != "+");
        GetComponent<TextMeshProUGUI>().text = valueStr;


        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.x = 15;

        Vector3 pos = GetComponent<RectTransform>().localPosition;
        pos.x = xPosition * size.x;
        GetComponent<RectTransform>().localPosition = pos;

        size.x *= value.ToString().Length;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isOpenTooltip = false;
        isMouseOver = true;
        _mouseOverCount = 0.0f;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _isOpenTooltip = false;
        isMouseOver = false;
        _mouseOverCount = 0.0f;

        _subTooltip.SetActive(false);
    }
}
