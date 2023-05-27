using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    private TurnSystem TurnSystem => CombatSystem.instance.turnSystem;
    private UnitSystem UnitSystem => CombatSystem.instance.unitSystem;

    public GameObject playerActionButtonsUI;
    public ActionButton[] actionArray;

    private ActionButton[] actionButtons => actionArray ??= GetComponentsInChildren<ActionButton>();

    void Awake()
    {
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
        if (CombatSystem.instance.IsPlayerTurn())
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
            actionButtons[index].SetAction(act);
            actionButtons[index].gameObject.SetActive(true);

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
}
