using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject TurnText;

    public TurnSystem turnSystem;

    public void SetTurnText(int currentTurn) 
    {
        TurnText.GetComponent<TextMeshProUGUI>().text = "Current Turn: " + currentTurn;
    }

    public override void OpenUI()
    {
    }
    public override void CloseUI()
    {
    }
}
