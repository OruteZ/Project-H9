using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject _TurnText;

    public void SetTurnText(int currentTurn) 
    {
        _TurnText.GetComponent<TextMeshProUGUI>().text = currentTurn + "ео";
    }
}
