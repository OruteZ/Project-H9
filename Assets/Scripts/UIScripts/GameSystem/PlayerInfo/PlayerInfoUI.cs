using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUI : UISystem
{
    public PlayerPortraitUI portraitUI;
    public PlayerExpUI expUI;
    public PlayerMainStatUI mainStatUI;
    public PlayerSummaryStatusUI summaryStatusUI;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.onPlayerStatChanged.AddListener(SetPlayerInfoUI);
        PlayerEvents.OnWeaponChanged.AddListener((weapon) => SetPlayerInfoUI());
        UIManager.instance.onSceneChanged.AddListener(SetPlayerInfoUI);
        //UIManager.instance.onTSceneChanged.AddListener(null);
        UIManager.instance.onActionChanged.AddListener(SetPlayerInfoUI);
    }

    private void SetPlayerInfoUI()
    {
        portraitUI.SetPortraitUI();
        mainStatUI.SetMainStatUI();
        summaryStatusUI.SetCurrentStatusUI();
    }
}
