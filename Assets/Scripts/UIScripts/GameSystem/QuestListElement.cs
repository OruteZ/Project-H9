using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class QuestListElement : UIElement, IPointerClickHandler
{
    [SerializeField] private GameObject _questNameText;
    [SerializeField] private GameObject _questDescriptText;
    [SerializeField] private GameObject _questRewardText;

    public QuestInfo currentQuestInfo { get; private set; }

    private bool _isDestroying = false;

    private string _resultText = "";
    private string _displayText = "";
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
            int itemStart = _resultText.IndexOf(_itemName);
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
            int skillStart = _resultText.IndexOf(_skillName);
            if (skillStart != -1 && tmpro.textInfo.characterInfo.Length > skillStart)
            {
                Vector3 skillBL = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[skillStart].bottomLeft;
                Vector3 skillTR = _questRewardText.GetComponent<RectTransform>().position + tmpro.textInfo.characterInfo[skillStart + _skillName.Length - 1].topRight;
                if (skillBL.x <= Input.mousePosition.x && Input.mousePosition.x < skillTR.x &&
                    skillBL.y <= Input.mousePosition.y && Input.mousePosition.y < skillTR.y)
                {
                    Vector3 pos = (skillBL + skillTR) / 2.0f;

                    UIManager.instance.gameSystemUI.questUI.OpenSkillTooltip(currentQuestInfo.SkillReward, pos);
                    return;
                }
            }
        }
        UIManager.instance.gameSystemUI.questUI.ClosePopupWindow();
    }

    public void SetQuestListElement(QuestInfo qInfo, out string popupStr)
    {
        currentQuestInfo = qInfo;
        _questNameText.GetComponent<TextMeshProUGUI>().text = qInfo.QuestName;

        string desc = "- " + qInfo.QuestTooltip;
        if (currentQuestInfo.ExpireTurn != -1) desc += " / " + UIManager.instance.UILocalization[205] + ": " + currentQuestInfo.CurTurn;
        PlayerEvents.OnProcessedWorldTurn.AddListener((t) => { ProgressTurnRemaining(); });
        _questDescriptText.GetComponent<TextMeshProUGUI>().text = desc;

        popupStr = UIManager.instance.UILocalization[201] + "\n" + qInfo.QuestName;

        string[] rewardTexts = { "", "", "", "" };

        _resultText = UIManager.instance.UILocalization[204] + ": ";
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
        if (qInfo.SkillReward > 0)
        { 
            _skillName = SkillManager.instance.GetSkillName(qInfo.SkillReward);
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
                    _resultText += ", ";
                }
                _resultText += rewardTexts[i];
            }
        }

        _displayText = _resultText;
        if (rewardTexts[2] != "") _displayText = _displayText.Replace(rewardTexts[2], UICustomColor.ChangeTextColor(rewardTexts[2], UICustomColor.ItemTextColor));
        if (rewardTexts[3] != "") _displayText = _displayText.Replace(rewardTexts[3], UICustomColor.ChangeTextColor(rewardTexts[3], UICustomColor.SkillTextColor));
        _questRewardText.GetComponent<TextMeshProUGUI>().text = _displayText;

        OpenUI();
    }
    private void ProgressTurnRemaining()
    {
        string desc = "- " + currentQuestInfo.QuestTooltip;
        if (currentQuestInfo.ExpireTurn != -1) desc += " / " + UIManager.instance.UILocalization[205] + ": " + currentQuestInfo.CurTurn;
        _questDescriptText.GetComponent<TextMeshProUGUI>().text = "- " + currentQuestInfo.QuestTooltip;
        _questDescriptText.GetComponent<TextMeshProUGUI>().text = desc;
    }
    public void FailQuestUI(out string popupStr)
    {
        popupStr = UIManager.instance.UILocalization[203];
        if (!_isDestroying)
        {
            _isDestroying = true;
            CloseUI();
            return;
        }
    }

    public void CompleteQuestUI(out string popupStr)
    {
        popupStr = UIManager.instance.UILocalization[202] + "\n" + _displayText;
        if (!_isDestroying)
        {
            _isDestroying = true;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CompleteQuestEffect());
            }
            else
            {
                CloseUI();
            }
            return;
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

        yield return new WaitForSeconds(1.5f);
        CloseUI();
        yield break;
    }
    public override void CloseUI()
    {
        PlayerEvents.OnProcessedWorldTurn.RemoveListener((t) => { ProgressTurnRemaining(); });
        base.CloseUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentQuestInfo.Pin != null)
        {
            Vector3Int pinPos = new Vector3Int(currentQuestInfo.Pin[0], currentQuestInfo.Pin[1], currentQuestInfo.Pin[2]);
            UIManager.instance.gameSystemUI.pinUI.SetPinUI(pinPos);
            UIManager.instance.gameSystemUI.pinUI.OnClickPin();
        }
    }
}
