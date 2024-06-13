using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlarmUITooltip : UIElement
{
    [SerializeField] private GameObject _alarmNameText;
    [SerializeField] private GameObject _alarmDescText;
    [SerializeField] private GameObject _alarmInteractText;
    
    public void SetAlarmTooltip(AlarmInfo info, Vector3 pos)
    {
        Vector3 position = GetComponent<RectTransform>().position;
        position.y = pos.y;
        GetComponent<RectTransform>().position = position;
        _alarmNameText.GetComponent<TextMeshProUGUI>().text = info.nameText;
        _alarmDescText.GetComponent<TextMeshProUGUI>().text = info.descriptionText;
        _alarmInteractText.GetComponent<TextMeshProUGUI>().text = info.interactText;
        OpenUI();
    }
}
