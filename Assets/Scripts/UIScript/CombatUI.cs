using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    public GameObject playerActionButtonsUI;
    public ActionButton[] actionButtonArray;

    void Awake()
    {
        actionButtonArray =  GetComponentsInChildren<ActionButton>(includeInactive: true);
    }

    void Start()
    {
        FieldSystem.turnSystem.onTurnChanged.AddListener(System_OnTurnChanged);
    }
    
    private void System_OnTurnChanged()
    {
        if (FieldSystem.turnSystem.turnOwner is Player && GameManager.instance.CompareState(GameState.Combat))
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

        var actions = FieldSystem.unitSystem.GetPlayer().GetUnitActionArray();

        int index = 0;
        foreach (var act in actions)
        {
            actionButtonArray[index].SetAction(act);
            actionButtonArray[index].gameObject.SetActive(true);

            index++;
        }
    }

    private void TurnOffPlayerActionUI()
    {
        if (!playerActionButtonsUI.activeSelf) return;
    
        foreach (var a in actionButtonArray)
        {
            a.gameObject.SetActive(false);
        }
        
        playerActionButtonsUI.SetActive(false);
    }
}
