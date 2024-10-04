using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlarmUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _alarmIcon;
    private AlarmInfo _alarmInfo;
    public void SetAlarmUIElement(AlarmInfo info)
    {
        SoundManager.instance.PlaySFX("UI_Alarm");
        _alarmInfo = info;
        _alarmIcon.GetComponent<Image>().sprite = info.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.gameSystemUI.alarmUI.OpenAlarmTooltip(_alarmInfo, GetComponent<RectTransform>().position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.gameSystemUI.alarmUI.CloseAlarmTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OpenRelatedUI();

            if (_alarmInfo.type == AlarmType.SkillPoint && SkillManager.instance.GetSkillPoint() > 0) return;
            if (_alarmInfo.type == AlarmType.StatPoint && UIManager.instance.gameSystemUI.playerStatLevelUpUI.GetPlayerStatPoint() > 0) return;
            CloseUI();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            CloseUI();
        }
    }
    private void OpenRelatedUI() 
    {
        switch (_alarmInfo.type)
        {
            case AlarmType.SkillPoint:
                {
                    if (UIManager.instance.GetSkillCanvasState()) return;
                    UIManager.instance.gameSystemUI.OnSkillBtnClick();
                    break;
                }
            case AlarmType.StatPoint:
                {
                    UIManager.instance.gameSystemUI.playerStatLevelUpUI.OpenPlayerStatLevelUpUI();
                    break;
                }
            case AlarmType.NewItem:
                {
                    if (UIManager.instance.GetCharacterCanvasState()) return;
                    UIManager.instance.SetCharacterCanvasState(false);
                    UIManager.instance.gameSystemUI.OnCharacterBtnClick();
                    break;
                }
            case AlarmType.NewSkill:
                {
                    if (UIManager.instance.GetSkillCanvasState()) return;
                    UIManager.instance.SetSkillCanvasState(false);
                    UIManager.instance.gameSystemUI.OnSkillBtnClick();
                    break;
                }
        }
    }
    public override void CloseUI()
    {
        UIManager.instance.gameSystemUI.alarmUI.CloseAlarmTooltip();
        Destroy(this.gameObject);
    }
}
