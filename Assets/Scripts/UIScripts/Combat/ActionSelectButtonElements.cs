using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ActionSelectButtonElements : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    public IUnitAction _action { get; private set; }
    private CombatActionUI _combatActionUI;

    private GameObject APCostUI;
    private GameObject AmmoCostUI;
    private Color APCostUIInitColor;
    private Color AmmoCostUIInitColor;

    private bool _isSelectable;

    // Start is called before the first frame update
    void Start()
    {
        APCostUI = gameObject.transform.GetChild(0).gameObject;
        AmmoCostUI = gameObject.transform.GetChild(1).gameObject;

        APCostUIInitColor = APCostUI.GetComponent<Image>().color;
        AmmoCostUIInitColor = AmmoCostUI.GetComponent<Image>().color;

        _isSelectable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_action!=null && _action.GetActionType() == FieldSystem.unitSystem.GetPlayer().GetSelectedAction().GetActionType()) 
        {
            Debug.Log("asdf");
        } 
    }
    public void SetActionSelectButton(IUnitAction action, Player player) 
    {
        _action = action;
        int apCost = action.GetCost();
        if (action.GetActionType() == ActionType.Move)
        {
            //fix later
            apCost = 1;
        }
        int ammoCost = GetAmmo();

        //Button selectable Setting
        _isSelectable = _action.IsSelectable();

        //Cost Icon Color Setting
        APCostUI.GetComponent<Image>().color = APCostUIInitColor;
        AmmoCostUI.GetComponent<Image>().color = AmmoCostUIInitColor;
        if (player.currentActionPoint < apCost)
        {
            APCostUI.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            _isSelectable = false;
        }
        if (player.weapon.currentEmmo < ammoCost)
        {
            AmmoCostUI.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            _isSelectable = false;
        }

        //Cost Icon Visible Setting
        APCostUI.SetActive(true);
        APCostUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = apCost.ToString();
        if (apCost == 0)
        {
            APCostUI.SetActive(false);
        }
        AmmoCostUI.SetActive(true);
        AmmoCostUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ammoCost.ToString();
        if (ammoCost == 0)
        {
            AmmoCostUI.SetActive(false);
        }

        //Button Color Setting
        GetComponent<Image>().color = new Color(1, 1, 1);
        if (action.GetActionType() == ActionType.Reload && player.weapon.currentEmmo == 0) 
        {
            GetComponent<Image>().color = new Color(1, 1, 0);
        }
        GetComponent<Button>().interactable = _isSelectable;
    }

    private int GetAmmo() 
    {
        if (_action.GetActionType() == ActionType.Attack) return 1;
        return 0;
    }
    public void OnClickActionSeleteButton() 
    {
        FieldSystem.unitSystem.GetPlayer().SelectAction(_action);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.combatUI.combatActionUI.ShowActionUITooltip(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.combatUI.combatActionUI.HideActionUITooltip();
    }
}
