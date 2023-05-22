using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    private TurnSystem TurnSystem => CombatManager.Instance.turnSystem;
    private UnitSystem UnitSystem => CombatManager.Instance.unitSystem;

    public GameObject playerActionButtonsUI;
    public ActionButton[] actionArray;

    private ActionButton[] ActionButtons => actionArray ??= GetComponentsInChildren<ActionButton>();

    void Awake()
    {
        UIManager.Instance.combatUI = this;
    }

    void Start()
    {
        TurnSystem.onTurnChanged.AddListener(System_OnTurnChanged);
    }

    public void EndTurnCall()
    {
        TurnSystem.EndTurn();
    }

    private void System_OnTurnChanged()
    {
        if (CombatManager.Instance.IsPlayerTurn())
        {
            TurnOnPlayerActionUI();
        }
        else
        {
            TurnOffPlayerActionUI();
        }
    }

    private void TurnOnPlayerActionUI()
    {
        playerActionButtonsUI.SetActive(true);

        var actions = UnitSystem.GetPlayer().GetUnitActionArray();

        int index = 0;
        foreach (var act in actions)
        {
            ActionButtons[index].SetAction(act);
            ActionButtons[index].gameObject.SetActive(true);

            index++;
        }
    }

    private void TurnOffPlayerActionUI()
    {
        foreach (var a in actionArray)
        {
            a.gameObject.SetActive(false);
        }
        
        playerActionButtonsUI.SetActive(false);
    }

    public void OnMouseEnter()
    {
        UIManager.Instance.isMouseOverUI = true;
    }

    public void OnMouseExit()
    {
        UIManager.Instance.isMouseOverUI = false;
    }
}
