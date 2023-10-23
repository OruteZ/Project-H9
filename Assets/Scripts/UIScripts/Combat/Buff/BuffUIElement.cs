using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUIElement : UIElement
{
    [SerializeField] private GameObject _buffImage;
    [SerializeField] private GameObject _buffEffect;
    [SerializeField] private GameObject _buffText;

    private int _currentSkillIndex;

    public void SetBuffUIElement(int skillIndex, bool isBuff, int duration) 
    {
        OpenUI();
        Debug.Log("UI On");
        _currentSkillIndex = skillIndex;
        /* buff image setting */

        if (isBuff)
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.BuffColor;
        }
        else
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.DebuffColor;
        }
        string durationText = duration.ToString();
        if (duration == 0) 
        {
            durationText = "";
        }
        _buffText.GetComponent<TextMeshProUGUI>().text = durationText;
    }
}
