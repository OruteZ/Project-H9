using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject _turnText;
    [SerializeField] private GameObject _turnOrder;

    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    public void SetTurnText(int currentTurn) 
    {
        _turnText.GetComponent<TextMeshProUGUI>().text = currentTurn + "ео";
    }
    public void SetTurnOrderUIState(bool isOn) 
    {
        _turnOrder.SetActive(isOn);
    }
}
