using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerMainStatElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _statText;
    [SerializeField] private GameObject _statTooltip;

    private string _statName = "";

    private void Awake()
    {
        _statTooltip.SetActive(false);
    }

    public void SetPlayerMainStatUI(string statName, string displayedText) 
    {
        _statName = statName;
        _statText.GetComponent<TextMeshProUGUI>().text = displayedText;
        _statTooltip.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _statTooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _statTooltip.SetActive(false);
    }
}
