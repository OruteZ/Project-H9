using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterTooltipText : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _subTooltip;
    [SerializeField] private GameObject _subTooltipText;

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

    public void SetCharacterTooltipText(string statName, UIStatType statType, string valueStr) 
    {
        OpenUI();
        if (statType == UIStatType.Base)
        {
            GetComponent<TextMeshProUGUI>().color = Color.white;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = Color.white;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[43];
            _isSetContents = true;
        }
        else if (statType == UIStatType.Character) 
        {
            GetComponent<TextMeshProUGUI>().color = UICustomColor.PlayerStatColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = UICustomColor.PlayerStatColor;
            string str = "";
            if (statName == "Additional Hit Rate" || statName == "Critical Chance" || statName == "Critical Damage")
            {
                ItemType playerWeaponType = FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType();
                if (playerWeaponType == ItemType.Revolver)
                {
                    str = UIManager.instance.UILocalization[44];
                }
                else if (playerWeaponType == ItemType.Repeater)
                {
                    str = UIManager.instance.UILocalization[45];
                }
                else if (playerWeaponType == ItemType.Shotgun)
                {
                    str = UIManager.instance.UILocalization[46];
                }
            }
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = str;
            _isSetContents = true;
        }
        else if (statType == UIStatType.Weapon)
        {
            GetComponent<TextMeshProUGUI>().color = UICustomColor.WeaponStatColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = UICustomColor.WeaponStatColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[47];
            _isSetContents = true;
        }
        else if (statType == UIStatType.Skill)
        {
            GetComponent<TextMeshProUGUI>().color = UICustomColor.SkillStatColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().color = UICustomColor.SkillStatColor;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[48];
            _isSetContents = true;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().color = Color.white;
            _subTooltipText.GetComponent<TextMeshProUGUI>().text = "";
            _isSetContents = false;
        }

        GetComponent<TextMeshProUGUI>().text = valueStr;
        GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
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
