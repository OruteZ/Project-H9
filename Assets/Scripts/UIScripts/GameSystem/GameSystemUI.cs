using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSystemUI : UISystem
{
    [SerializeField] private GameObject _skillButtonRedDot;
    public PlayerInfoUI playerInfoUI { get; private set; }
    public QuestUI questUI { get; private set; }
    public TurnUI turnUI { get; private set; }
    public PlayerStatLevelUpUI playerStatLevelUpUI { get; private set; }
    public ConversationUI conversationUI { get; private set; }
    public PinUI pinUI { get; private set; }
    public TownUI townUI { get; private set; }
    public LinkInfoUI linkInfoUI { get; private set; }
    public AlarmUI alarmUI { get; private set; }
    public ItemUI itemUI { get; private set; }
    public EquipmentUI equipmentUI { get; private set; }
    public TargetListUI targetListUI { get; private set; }
    public UnitSpeechUI speechUI { get; private set; }

    private void Awake()
    {
        playerInfoUI = GetComponent<PlayerInfoUI>();
        questUI = GetComponent<QuestUI>();
        turnUI = GetComponent<TurnUI>();
        playerStatLevelUpUI = GetComponent<PlayerStatLevelUpUI>();
        conversationUI = GetComponent<ConversationUI>();
        pinUI = GetComponent<PinUI>();
        townUI = GetComponent<TownUI>();
        linkInfoUI = GetComponent<LinkInfoUI>();
        alarmUI = GetComponent<AlarmUI>();
        itemUI = GetComponent<ItemUI>();
        equipmentUI = GetComponent<EquipmentUI>();
        targetListUI = GetComponent<TargetListUI>();
        speechUI = GetComponent<UnitSpeechUI>();

        uiSubsystems.Add(playerInfoUI);
        uiSubsystems.Add(questUI);
        uiSubsystems.Add(turnUI);
        uiSubsystems.Add(playerStatLevelUpUI);
        uiSubsystems.Add(conversationUI);
        uiSubsystems.Add(pinUI);
        uiSubsystems.Add(townUI);
        uiSubsystems.Add(linkInfoUI);
        uiSubsystems.Add(alarmUI);
        uiSubsystems.Add(itemUI);
        uiSubsystems.Add(equipmentUI);
        uiSubsystems.Add(targetListUI);
        uiSubsystems.Add(speechUI);

        ChangeSkillButtonRedDotText(SkillManager.instance.GetSkillPoint());
        UIManager.instance.onTSceneChanged.AddListener((s) => { ChangeSkillButtonRedDotText(SkillManager.instance.GetSkillPoint()); });
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
        if (Input.GetKeyDown(HotKey.openTargetListKey))
        {
            targetListUI.OnClickTargetListButton();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))    //level up test button
        {
            playerStatLevelUpUI.AddPlayerStatPoint();
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))    //skill test button
        {
            playerStatLevelUpUI.AddPlayerStatPoint();
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

    public void ChangeSkillButtonRedDotText(int sp) 
    {
        string t = sp.ToString();
        //if (sp == 1) t = "";
        _skillButtonRedDot.SetActive(sp != 0);
        _skillButtonRedDot.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = t;
    }
}
