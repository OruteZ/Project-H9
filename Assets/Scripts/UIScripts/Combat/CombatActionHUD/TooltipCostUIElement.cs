using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipCostUIElement : UIElement
{
    [SerializeField] private GameObject _costIcon;
    [SerializeField] private GameObject _costIconFrame;
    [SerializeField] private GameObject _costText;

    public void SetTooltipCostUIElement(string costName, int cost, bool isEnough) 
    {
        if (cost == 0)
        {
            CloseUI();
        }
        else 
        {
            OpenUI();
        }
        Sprite spr = null;
        if (costName == "Ap Cost")
        {
            spr = UIManager.instance.iconDB.GetIconInfo("Action Point");
        }
        else if (costName == "Ammo Cost") 
        {
            spr = UIManager.instance.iconDB.GetIconInfo("Ammo");
        }
        _costIcon.GetComponent<Image>().sprite = spr;
        //_costIconFrame.GetComponent<Image>().sprite = spr;

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
