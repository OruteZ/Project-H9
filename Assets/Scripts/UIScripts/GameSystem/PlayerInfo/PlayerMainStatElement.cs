using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerMainStatElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _statIcon;
    [SerializeField] private GameObject _statText;
    public string statName { get; private set; }

    private void Awake()
    {
        statName = "";
    }

    public void SetPlayerMainStatUI(string statName, string displayedText) 
    {
        this.statName = statName;
        _statIcon.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo(statName);
        _statText.GetComponent<TextMeshProUGUI>().text = displayedText;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.gameSystemUI.playerInfoUI.mainStatUI.GetComponent<PlayerMainStatUI>().ShowMainStatTooltip(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.gameSystemUI.playerInfoUI.mainStatUI.GetComponent<PlayerMainStatUI>().HideMainStatTooltip();
    }
}
