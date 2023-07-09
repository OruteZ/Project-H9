using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimingUI : UISystem
{
    [SerializeField] private GameObject TurnText;

    public TurnSystem turnSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetTurnText(turnSystem.turnNumber);
    }

    public void SetTurnText(int currentTurn) 
    {
        TurnText.GetComponent<TextMeshProUGUI>().text = "Current Turn: " + currentTurn.ToString();
    }

    public override void OpenUI()
    {
    }
    public override void CloseUI()
    {
    }
}
