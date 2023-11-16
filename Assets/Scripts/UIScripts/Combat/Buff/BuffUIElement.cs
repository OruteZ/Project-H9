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

    public int skillIndex { get; private set; }

    public void SetBuffUIElement(IDisplayableEffect effect, bool isBuff) 
    {
        OpenUI();
        Debug.Log("UI On");
        skillIndex = effect.GetIndex();
        /* buff image setting */

        if (isBuff)
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.BuffColor;
        }
        else
        {
            _buffEffect.GetComponent<Image>().color = UICustomColor.DebuffColor;
        }
        string durationText = effect.GetDuration().ToString();
        if (effect.GetDuration() <= 0 || effect.GetDuration() >= 100) 
        {
            durationText = "";
        }
        _buffText.GetComponent<TextMeshProUGUI>().text = durationText;
    }
}
