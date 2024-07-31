using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TargetQuestListElement : QuestListElement
{
    private void LateUpdate()
    {
        
    }
    public override void SetQuestListElement(QuestInfo qInfo, out string popupStr)
    {
        currentQuestInfo = qInfo;
        if (qInfo == null) CloseUI();

        _questNameText.GetComponent<TextMeshProUGUI>().text = qInfo.QuestName;

        string desc = "- " + qInfo.QuestTooltip;
        _questDescriptText.GetComponent<TextMeshProUGUI>().text = desc;


        if (GameManager.instance.user.ClearedQuests.Contains(qInfo.Index))
        {
            _questNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.QuestClearTextColor;
            _questDescriptText.GetComponent<TextMeshProUGUI>().color = UICustomColor.QuestClearTextColor;
        }
        else if (UIManager.instance.gameSystemUI.questUI.GetCurrentProgressingQuests.Contains(qInfo))
        {
            _questNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.QuestNameColor;
            _questDescriptText.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
#if UNITY_EDITOR
            _questNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.QuestDisableTextColor;
            _questDescriptText.GetComponent<TextMeshProUGUI>().color = UICustomColor.QuestDisableTextColor;
#else
            CloseUI();
#endif
        }

        popupStr = "";
        OpenUI();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }
}
