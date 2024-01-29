using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerConcentrationUI : UIElement
{
    [SerializeField] private GameObject _concentrationFill;
    [SerializeField] private GameObject _concentrationText;

    private const float MAX_CONCENTRATION = 120.0f;
    public void SetConcentrationUI() 
    {
        int concentration = GameManager.instance.playerStat.concentration;
        _concentrationFill.GetComponent<Image>().fillAmount = concentration / MAX_CONCENTRATION;
        _concentrationText.GetComponent<TextMeshProUGUI>().text = concentration.ToString();
    }
}
