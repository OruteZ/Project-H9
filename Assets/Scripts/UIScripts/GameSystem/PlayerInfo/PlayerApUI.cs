using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerApUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _apElements;
    [SerializeField] private GameObject _apTooltip;
    void Start()
    {
        _apTooltip?.SetActive(false);
    }
    public void SetApUI()
    {
        int maxAp = GameManager.instance.playerStat.maxActionPoint;
        int curAp = FieldSystem.unitSystem.GetPlayer().currentActionPoint;
        int flickerCnt = UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage;
        bool isExist = true;
        bool isFilled = true;
        bool isFlickering = false;
        for (int i = 0; i < _apElements.transform.childCount; i++)
        {
            if (i >= curAp - flickerCnt) isFlickering = true;
            if (i >= curAp) isFilled = false;
            if (i >= maxAp) isExist = false;

            _apElements.transform.GetChild(i).GetComponent<PlayerApUIElement>().SetApUIElement(isExist, isFilled, isFlickering);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _apTooltip?.GetComponent<PlayerMainStatTooltip>().SetPlayerMainStatTooltip(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _apTooltip?.SetActive(false);
    }
}
