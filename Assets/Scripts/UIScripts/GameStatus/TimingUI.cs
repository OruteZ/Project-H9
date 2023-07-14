using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject TurnText;

    public void SetTurnText(int currentTurn) 
    {
        TurnText.GetComponent<TextMeshProUGUI>().text = currentTurn + "ео";
    }
}
