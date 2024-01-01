using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerSummaryStatElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _statText;
    [SerializeField] private GameObject _statTooltip;

    private string _statName;

    private void Awake()
    {
        _statTooltip.SetActive(false);
    }

    public void SetPlayerSummaryUI(string statName, string displayedText) 
    {
        _statName = statName;

        _statText.GetComponent<TextMeshProUGUI>().text = displayedText;
        _statTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _statName;
        _statTooltip.SetActive(false);

        if (statName == "Action Point")
        {
            Player player = FieldSystem.unitSystem.GetPlayer();
            if (player is null) return;
            if (player.currentActionPoint == 0)
            {
                _statText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                //_statText.GetComponent<TextMeshProUGUI>().color = Color.white;
                _statText.GetComponent<TextMeshProUGUI>().color = UICustomColor.ActionAPColor;
            }
        }
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
