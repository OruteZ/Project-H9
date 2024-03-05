using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionSkillCostUIElement : UIElement
{
    //[SerializeField] private GameObject _costIcon;
    [SerializeField] private GameObject _costText;

    public void SetTooltipCostUIElement(int cost, bool isEnough) 
    {
        if (cost == 0)
        {
            CloseUI();
        }
        else 
        {
            OpenUI();
        }
        _costText.GetComponent<TextMeshProUGUI>().text = "x " + cost.ToString();
        if (isEnough)
        {
            _costText.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else 
        {
            _costText.GetComponent<TextMeshProUGUI>().color = UICustomColor.SkillStatColor;
        }
    }
}
