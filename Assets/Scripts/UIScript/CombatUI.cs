using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    public TurnSystem turnSystem => CombatSystem.instance.turnSystem;
    public UnitSystem unitSystem => CombatSystem.instance.unitSystem;

    public GameObject playerActionButtonsUI;
    public ActionButton[] actionArray;

    private ActionButton[] _actionButtons => actionArray ??= GetComponentsInChildren<ActionButton>();

    void Awake()
    {
    }

    void Start()
    {
        turnSystem.onTurnChanged.AddListener(System_OnTurnChanged);
    }

    public void EndTurnCall()
    {
        turnSystem.EndTurn();
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

        var actions = unitSystem.GetPlayer().GetUnitActionArray();

        int index = 0;
        foreach (var act in actions)
        {
            _actionButtons[index].SetAction(act);
            _actionButtons[index].gameObject.SetActive(true);

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
