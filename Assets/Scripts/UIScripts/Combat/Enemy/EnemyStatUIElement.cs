using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EnemyStatUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _statText;
    private string _statName = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.combatUI.enemyStatUI.OpenStatTooltip(GetComponent<RectTransform>().position, _statName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.combatUI.enemyStatUI.CloseStatTooltip();
    }

    public void SetEnemyStatUIElement(string name, int value) 
    {
        _statName = name;
        _statText.GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}
