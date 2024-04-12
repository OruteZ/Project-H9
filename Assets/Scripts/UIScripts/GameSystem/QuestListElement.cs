using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class QuestListElement : UIElement
{
    [SerializeField] private GameObject _questNameText;
    [SerializeField] private GameObject _questDescriptText;
    [SerializeField] private GameObject _questRewardText;

    public QuestInfo currentQuestInfo { get; private set; }

    private bool _isDestroying = false;

    private string _resultString = "";
    private string _itemName = "";
    private string _skillName = "";

    void Awake()
    {
        currentQuestInfo = null;
    }
    private void LateUpdate()
    {
        if (!gameObject.activeSelf) return;
        if (currentQuestInfo == null) return;
        TextMeshProUGUI tmpro = _questRewardText.GetComponent<TextMeshProUGUI>();
        if (_itemName != "")
        {
            int itemStart = _resultString.IndexOf(_itemName);
            if (itemStart != -1 && tmpro.textInfo.characterInfo.Length > itemStart)
            {
                Vector3 itemBL = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[itemStart].bottomLeft;
                Vector3 itemTR = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[itemStart + _itemName.Length - 1].topRight;
                if (itemBL.x <= Input.mousePosition.x && Input.mousePosition.x < itemTR.x &&
                    itemBL.y <= Input.mousePosition.y && Input.mousePosition.y < itemTR.y)
                {
                    Vector3 pos = (itemBL + itemTR) / 2.0f;

                    UIManager.instance.gameSystemUI.questUI.OpenItemTooltip(currentQuestInfo.ItemReward, pos);
                    return;
                }
            }
        }
        if (_skillName != "")
        {
            int skillStart = _resultString.IndexOf(_skillName);
            if (skillStart != -1 && tmpro.textInfo.characterInfo.Length > skillStart)
            {
                Vector3 skillBL = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[skillStart].bottomLeft;
                Vector3 skillTR = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[skillStart + _skillName.Length - 1].topRight;
                if (skillBL.x <= Input.mousePosition.x && Input.mousePosition.x < skillTR.x &&
                    skillBL.y <= Input.mousePosition.y && Input.mousePosition.y < skillTR.y)
                {
                    Vector3 pos = (skillBL + skillTR) / 2.0f;

                    UIManager.instance.gameSystemUI.questUI.OpenSkillTooltip(currentQuestInfo.SKillReward, pos);
                    return;
                }
            }
        }
        UIManager.instance.gameSystemUI.questUI.ClosePopupWindow();
    }

    public void SetQuestListElement(QuestInfo qInfo)
    {
        currentQuestInfo = qInfo;
        _questNameText.GetComponent<TextMeshProUGUI>().text = qInfo.QuestName;
        _questDescriptText.GetComponent<TextMeshProUGUI>().text = "- " + qInfo.QuestTooltip;
        string[] rewardTexts = { "", "", "", "" };

        _resultString = "Reward: ";
        if (qInfo.MoneyReward > 0)
        {
            rewardTexts[0] = qInfo.MoneyReward.ToString() + "$";
        }
        if (qInfo.ExpReward > 0)
        {
            rewardTexts[1] = qInfo.ExpReward.ToString() + "Exp";
        }
        if (qInfo.ItemReward > 0)
        {
            _itemName = GameManager.instance.itemDatabase.GetItemScript(qInfo.ItemReward).GetName();
            rewardTexts[2] = _itemName;
        }
        if (qInfo.MoneyReward > 0)
        { 
            _skillName = SkillManager.instance.GetSkillName(qInfo.SKillReward);
            rewardTexts[3] = _skillName;
        }
        bool isFirstReward = true;
        for (int i = 0; i < rewardTexts.Length; i++) 
        {
            if (rewardTexts[i] != "") 
            {
                if (isFirstReward)
                {
                    isFirstReward = !isFirstReward;
                }
                else 
                {
                    _resultString += ", ";
                }
                _resultString += rewardTexts[i];
            }
        }

        string displayedText = _resultString;
        if (rewardTexts[2] != "") displayedText = displayedText.Replace(rewardTexts[2], UICustomColor.ChangeTextColor(rewardTexts[2], UICustomColor.ItemTextColor));
        if (rewardTexts[3] != "") displayedText = displayedText.Replace(rewardTexts[3], UICustomColor.ChangeTextColor(rewardTexts[3], UICustomColor.SkillTextColor));
        _questRewardText.GetComponent<TextMeshProUGUI>().text = displayedText;

        OpenUI();
    }

    public void CompleteQuestUI() 
    {
        if (!_isDestroying) 
        {
            _isDestroying = true;
            StartCoroutine(CompleteQuestEffect());
        }
    }
    IEnumerator CompleteQuestEffect() 
    {
        _questNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.SkillIconLearnedColor;
        _questDescriptText.GetComponent<TextMeshProUGUI>().color = UICustomColor.SkillIconLearnedColor;
        //_questRewardText.GetComponent<TextMeshProUGUI>().color = UICustomColor.DisableStateColor;
        //_questDescriptText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
        //_questDescriptText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        //_questRewardText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
        //_questRewardText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;

        yield return new WaitForSeconds(2.0f);
        CloseUI();
        yield break;
    }
}
