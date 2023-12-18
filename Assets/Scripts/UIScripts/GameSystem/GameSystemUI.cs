using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemUI : UISystem
{
    public PlayerSummaryStatUI playerSummaryStatUI { get; private set; }
    public PlayerHpUI playerHpUI { get; private set; }
    public PlayerExpUI playerExpUI { get; private set; }
    public QuestUI questUI { get; private set; }
    public TurnUI turnUI { get; private set; }
    public PlayerStatLevelUpUI playerStatLevelUpUI { get; private set; }

    private void Awake()
    {
        playerSummaryStatUI = GetComponent<PlayerSummaryStatUI>();
        playerHpUI = GetComponent<PlayerHpUI>();
        playerExpUI = GetComponent<PlayerExpUI>();
        questUI = GetComponent<QuestUI>();
        turnUI = GetComponent<TurnUI>();
        playerStatLevelUpUI = GetComponent<PlayerStatLevelUpUI>();

        uiSubsystems.Add(playerSummaryStatUI);
        uiSubsystems.Add(playerHpUI);
        uiSubsystems.Add(playerExpUI);
        uiSubsystems.Add(questUI);
        uiSubsystems.Add(turnUI);
        uiSubsystems.Add(playerStatLevelUpUI);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnCharacterBtnClick();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
#if UNITY_EDITOR
            playerStatLevelUpUI.OpenPlayerStatLevelUpUI();
#endif
        }
    }

    public void OnCharacterBtnClick()
    {
        UIManager.instance.SetCharacterCanvasState(true);
        UIManager.instance.SetSkillCanvasState(false);
        UIManager.instance.SetPauseMenuCanvasState(false);
    }
    public void OnSkillBtnClick()
    {
        UIManager.instance.SetCharacterCanvasState(false);
        UIManager.instance.SetSkillCanvasState(true);
        UIManager.instance.SetPauseMenuCanvasState(false);
    }
    public void OnPauseMenuBtnClick()
    {
        UIManager.instance.SetCharacterCanvasState(false);
        UIManager.instance.SetSkillCanvasState(false);
        UIManager.instance.SetPauseMenuCanvasState(true);
    }
}
