using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestListElement : UIElement
{
    [SerializeField] private GameObject _questNameText;
    [SerializeField] private GameObject _questDescriptText;
    [SerializeField] private GameObject _questRewardText;

    public int questIndex { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        questIndex = -1;
    }

    public void SetQuestListElement() 
    {
        //questIndex = quest.index;
        //_questNameText.GetComponent<TextMeshProUGUI>().text = quest.name;
        //_questDescriptText.GetComponent<TextMeshProUGUI>().text = quest.desc;
        //_questRewardText.GetComponent<TextMeshProUGUI>().text = quest.reward;

        OpenUI();
    }

    public void CompleteQuestUI() 
    {
        StartCoroutine(CompleteQuestEffect());
    }
    IEnumerator CompleteQuestEffect() 
    {
        _questNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.PlayerColor;
        _questDescriptText.GetComponent<TextMeshProUGUI>().color = UICustomColor.DisableStateColor;
        _questRewardText.GetComponent<TextMeshProUGUI>().color = UICustomColor.DisableStateColor;
        _questDescriptText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
        _questRewardText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;

        yield return new WaitForSeconds(2.0f);
        CloseUI();
        UIManager.instance.gameSystemUI.questUI.SetQuestListUI();
        yield break;
    }
}
