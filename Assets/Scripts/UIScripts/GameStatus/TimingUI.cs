using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject _turnText;
    [SerializeField] private GameObject _turnOrder;
    private readonly Vector3 TURN_ORDER_UI_INIT_POSITION = new Vector3(0, 0, 0);
    private const int TURN_ORDER_UI_INTERVAL = 10;

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
    public void SetTurnOrderUI(List<Unit> turnOrder) 
    {
        foreach(Unit unit in turnOrder) 
        {
            Debug.Log(unit.unitName);
        }
    }
}
