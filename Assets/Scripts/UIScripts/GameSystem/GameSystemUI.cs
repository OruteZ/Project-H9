using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemUI : UISystem
{
    public PlayerInfoUI playerInfoUI { get; private set; }
    public QuestUI questUI { get; private set; }
    public TurnUI turnUI { get; private set; }
    public PlayerStatLevelUpUI playerStatLevelUpUI { get; private set; }

    private void Awake()
    {
        playerInfoUI = GetComponent<PlayerInfoUI>();
        questUI = GetComponent<QuestUI>();
        turnUI = GetComponent<TurnUI>();
        playerStatLevelUpUI = GetComponent<PlayerStatLevelUpUI>();

        uiSubsystems.Add(playerInfoUI);
        uiSubsystems.Add(questUI);
        uiSubsystems.Add(turnUI);
        uiSubsystems.Add(playerStatLevelUpUI);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(HotKey.openCharacterUIKey))
        {
            OnCharacterBtnClick();
        }
        if (Input.GetKeyDown(HotKey.openSkillUIKey))
        {
            OnSkillBtnClick();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))    //level up test button
        {
            playerStatLevelUpUI.OpenPlayerStatLevelUpUI();
        }
#endif
    }

    public void OnCharacterBtnClick()
    {
        if (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive()) return;
        UIManager.instance.SetCharacterCanvasState(true);
    }
    public void OnSkillBtnClick()
    {
        if (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive()) return;
        UIManager.instance.SetSkillCanvasState(true);
    }
    public void OnPauseMenuBtnClick()
    {
        if (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive()) return;
        UIManager.instance.SetPauseMenuCanvasState(true);
    }
}
