using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CombatActionButtonElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _actionButton;
    [SerializeField] private GameObject _actionButtonIcon;
    [SerializeField] private GameObject _actionButtonNumber;

    [SerializeField] private CombatActionType _actionType = CombatActionType.Null;
    private int buttonIndex = 0;
    public string buttonName { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (_actionType != CombatActionType.Null) 
        {
            //_actionButtonIcon.GetComponent<Image>().sprite = ;
            _actionButtonNumber.GetComponent<TextMeshProUGUI>().text = ((int)_actionType).ToString();
            buttonName = _actionType.ToString();
        }
    }

    public void SetcombatActionButton(CombatActionType actionType, int btnNumber, ActionType at) 
    {
        _actionType = actionType;
        buttonIndex = btnNumber;
        _actionButtonNumber.GetComponent<TextMeshProUGUI>().text = (btnNumber + 1).ToString();
        buttonName = at.ToString();
    }

    public void OnClickCombatSelectButton()
    {
        UIManager.instance.combatUI.combatActionUI.TakeAction(_actionType, buttonIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.combatUI.combatActionUI.ShowActionUITooltip(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.combatUI.combatActionUI.HideActionUITooltip();
    }
}
