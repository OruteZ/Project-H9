using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlarmUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _alarmIcon;
    private AlarmInfo _alarmInfo;
    public bool isSettingPosition = false;
    private Vector3 _curChildPosition= Vector3.zero;
    private float _initX;

    public void SetAlarmUIElement(AlarmInfo info)
    {
        SoundManager.instance.PlaySFX("UI_Alarm");
        _alarmInfo = info;
        _alarmIcon.GetComponent<Image>().sprite = info.icon;

        _initX = GetComponent<RectTransform>().position.x;
        transform.GetChild(0).GetComponent<RectTransform>().position = GetComponent<RectTransform>().position - new Vector3(150, 0, 0);
        isSettingPosition = true;
    }

    private void Update()
    {
        if (isSettingPosition)
        {
            if (_curChildPosition == Vector3.zero) _curChildPosition = transform.GetChild(0).GetComponent<RectTransform>().position;
            LerpCalculation.CalculateLerpValue(ref _curChildPosition, GetComponent<RectTransform>().position, 5);
            transform.GetChild(0).GetComponent<RectTransform>().position = _curChildPosition;
        }
        if (_curChildPosition.x <= _initX - 150)
        {
            Destroy(this.gameObject);
        }
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
        transform.SetParent(UIManager.instance.gameSystemUI.gameObject.transform);

        isSettingPosition = true;
        _curChildPosition = GetComponent<RectTransform>().position;
        GetComponent<RectTransform>().position -= new Vector3(200, 0, 0);

        UIManager.instance.gameSystemUI.alarmUI.CloseAlarmElement(this);
    }
}
