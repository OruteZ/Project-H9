using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerConcentrationUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _concentrationFill;
    [SerializeField] private GameObject _concentrationText;

    [SerializeField] private GameObject _concentrationTooltip;

    private const float MAX_CONCENTRATION = 120.0f;
    void Start()
    {
        _concentrationTooltip?.SetActive(false);
    }
    public void SetConcentrationUI() 
    {
        int concentration = GameManager.instance.playerStat.concentration;
        _concentrationFill.GetComponent<Image>().fillAmount = concentration / MAX_CONCENTRATION;
        _concentrationText.GetComponent<TextMeshProUGUI>().text = concentration.ToString();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _concentrationTooltip.GetComponent<PlayerMainStatTooltip>().SetPlayerMainStatTooltip(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _concentrationTooltip?.SetActive(false);
    }
}
