using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUI : UISystem
{
    [SerializeField] GameObject _alarmUIElements;
    [SerializeField] GameObject _alarmTooltip;

    [SerializeField] GameObject _newSkillAlarmPrefab;
    [SerializeField] GameObject _newItemAlarmPrefab;
    [SerializeField] GameObject _statPointAlarmPrefab;
    [SerializeField] GameObject _skillPointAlarmPrefab;

    public Sprite defaultImage;

    private GameObject _skillPointAlarm = null;
    private GameObject _statPointAlarm = null;

    private const int MAX_ALARM_COUNT = 6;

    private List<int> _earnedWeaponIndex = new List<int>();

    private void Awake()
    {
        CloseUI();
        UIManager.instance.onTSceneChanged.AddListener((gs) => { if (gs == GameState.WORLD) CloseUI(); });

        ClearAlarmUI();
    }
    private void Start()
    {
        if (_skillPointAlarm == null && SkillManager.instance.GetSkillPoint() > 0) AddAlarmUI(AlarmType.SkillPoint);
        if (_statPointAlarm == null && UIManager.instance.gameSystemUI.playerStatLevelUpUI.GetPlayerStatPoint() > 0) AddAlarmUI(AlarmType.StatPoint);
    }
    public void ClearAlarmUI()
    {
        for (int i = _alarmUIElements.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_alarmUIElements.transform.GetChild(i).gameObject);
        }
    }
    public override void CloseUI() 
    {
        _alarmTooltip.SetActive(false);
    }

    public void AddAlarmUI(Skill skill)
    {
        Sprite sprite = skill.skillInfo.icon;
        if (sprite == null)
        {
            Debug.LogError(skill.skillInfo.index + "번 스킬의 스프라이트 " + skill.skillInfo.icon + "를 찾을 수 없습니다.");
            sprite = defaultImage;
        }
        AlarmInfo info = new AlarmInfo
        (
            AlarmType.NewSkill,
            sprite,
            UIManager.instance.UILocalization[1010],
            UIManager.instance.UILocalization[1011],
            UIManager.instance.UILocalization[1012] + '\n' + UIManager.instance.UILocalization[1000]
        );

        CreateAlarmUI(info);
    }
    public void AddAlarmUI(WeaponItem item)
    {
        foreach (int index in _earnedWeaponIndex) 
        {
            if (item.GetData().id == index) return;
        }
        _earnedWeaponIndex.Add(item.GetData().id);

        Sprite sprite = item.GetData().icon;
        if (sprite == null)
        {
            Debug.LogError(item.GetData().id + "번 무기의 스프라이트를 찾을 수 없습니다.");
            sprite = defaultImage;
        }
        AlarmInfo info = new AlarmInfo
        (
            AlarmType.NewItem,
            sprite,
            UIManager.instance.UILocalization[1007],
            UIManager.instance.UILocalization[1008],
            UIManager.instance.UILocalization[1009] + '\n' + UIManager.instance.UILocalization[1000]
        );

        CreateAlarmUI(info);
    }
    public void AddAlarmUI(AlarmType alarmType)
    {
        AlarmInfo info = new AlarmInfo();
        if (SkillManager.instance!= null && alarmType == AlarmType.SkillPoint && SkillManager.instance.GetSkillPoint() > 0)
        {
            info = new AlarmInfo
            (
                alarmType,
                UIManager.instance.iconDB.GetIconInfo("SkillAlarm"),
                UIManager.instance.UILocalization[1001],
                UIManager.instance.UILocalization[1002],
                UIManager.instance.UILocalization[1003] + '\n' + UIManager.instance.UILocalization[1000]
            );
        }
        else if (alarmType == AlarmType.StatPoint)
        {
            info = new AlarmInfo
            (
                alarmType,
                UIManager.instance.iconDB.GetIconInfo("StatAlarm"),
                UIManager.instance.UILocalization[1004],
                UIManager.instance.UILocalization[1005],
                UIManager.instance.UILocalization[1006] + '\n' + UIManager.instance.UILocalization[1000]
            );
        }

        CreateAlarmUI(info);
    }
    public void DeleteAlarmUI(AlarmType alarmType)
    {
        if (alarmType == AlarmType.SkillPoint && _skillPointAlarm != null)
        {
            _skillPointAlarm.GetComponent<AlarmUIElement>().CloseUI();
        }
        else if (alarmType == AlarmType.StatPoint && _statPointAlarm != null)
        {
            _statPointAlarm.GetComponent<AlarmUIElement>().CloseUI();
        }
    }



    private void CreateAlarmUI(AlarmInfo info)
    {
        List<GameObject> deletableObjects = new List<GameObject>();
        for (int i = 0; i < _alarmUIElements.transform.childCount; i++)
        {
            if (_alarmUIElements.transform.GetChild(i).gameObject == _skillPointAlarm) continue;
            if (_alarmUIElements.transform.GetChild(i).gameObject == _statPointAlarm) continue;
            deletableObjects.Add(_alarmUIElements.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _alarmUIElements.transform.childCount - MAX_ALARM_COUNT; i++)
        {
            DestroyImmediate(deletableObjects[i]);
        }

        GameObject ui = null;
        switch (info.type) 
        {
            case AlarmType.NewSkill:
                {
                    ui = Instantiate(_newSkillAlarmPrefab, _alarmUIElements.transform);
                    ui.transform.SetAsLastSibling();
                    break;
                }
            case AlarmType.NewItem:
                {
                    ui = Instantiate(_newItemAlarmPrefab, _alarmUIElements.transform);
                    ui.transform.SetAsLastSibling();
                    break;
                }
            case AlarmType.SkillPoint: 
                {
                    if (_skillPointAlarm != null) return;

                    ui = Instantiate(_skillPointAlarmPrefab, _alarmUIElements.transform);
                    ui.transform.SetAsFirstSibling();
                    _skillPointAlarm = ui;
                    break;
                }
            case AlarmType.StatPoint:
                {
                    if (_statPointAlarm != null) return;

                    ui = Instantiate(_statPointAlarmPrefab, _alarmUIElements.transform);
                    ui.transform.SetAsFirstSibling();
                    if (_skillPointAlarm != null)
                    {
                        _skillPointAlarm.transform.SetAsFirstSibling();
                    }
                    _statPointAlarm = ui;
                    break;
                }
        }
        if (ui is null) return;
        ui.GetComponent<AlarmUIElement>().SetAlarmUIElement(info);
        ui.SetActive(true);
    } 

    internal void OpenAlarmTooltip(AlarmInfo alarmInfo, Vector3 pos)
    {
        _alarmTooltip.GetComponent<AlarmUITooltip>().SetAlarmTooltip(alarmInfo, pos);
    }
    internal void CloseAlarmTooltip()
    {
        _alarmTooltip.GetComponent<AlarmUITooltip>().CloseUI();
    }

    internal void CloseAlarmElement(AlarmUIElement uIElement)
    {
        for (int i = 0; i < _alarmUIElements.transform.childCount; i++)
        {
            if (uIElement == _alarmUIElements.transform.GetChild(i).GetComponent<AlarmUIElement>()) continue;
            _alarmUIElements.transform.GetChild(i).GetComponent<AlarmUIElement>().isSettingPosition = true;
        }
        _alarmTooltip.GetComponent<AlarmUITooltip>().CloseUI();
    }
}

public struct AlarmInfo
{
    public AlarmType type;
    public Sprite icon;
    public string nameText;
    public string descriptionText;
    public string interactText;

    public AlarmInfo(AlarmType t, Sprite s, string a, string b, string c) 
    {
        type = t;
        icon = s;
        nameText = a;
        descriptionText = b;
        interactText = c;
    }
}
public enum AlarmType 
{
    NULL,
    SkillPoint,
    StatPoint,
    NewItem,
    NewSkill
}
