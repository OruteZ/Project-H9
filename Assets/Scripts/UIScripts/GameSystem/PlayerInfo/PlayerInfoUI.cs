using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUI : UISystem
{
    public GameObject portraitUI;
    public GameObject expUI;
    public GameObject statUI;
    public GameObject statusUI;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.onPlayerStatChanged.AddListener(SetPlayerInfoUI);
        UIManager.instance.onTurnChanged.AddListener(SetPlayerInfoUI);
        UIManager.instance.onActionChanged.AddListener(SetPlayerInfoUI);
    }

    private void SetPlayerInfoUI()
    {
        portraitUI.GetComponent<PlayerPortraitUI>().SetPortraitUI();
        statUI.GetComponent<PlayerMainStatUI>().SetMainStatUI();
        statusUI.GetComponent<PlayerSummaryStatusUI>().SetCurrentStatusUI();
    }
}
