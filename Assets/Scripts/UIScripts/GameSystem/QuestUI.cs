using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 수주한 퀘스트를 표시하는 기능을 구현한 클래스
/// </summary>
public class QuestUI : UISystem
{
    [SerializeField] private GameObject _questWindow;
    [SerializeField] private GameObject _listElementContainer;
    [SerializeField] private GameObject _itemTooltip;
    [SerializeField] private GameObject _skillTooltip;
    [SerializeField] private GameObject _rewardWindow;
    [SerializeField] private GameObject _rewardText;

    private QuestListPool _listPool = null;

    void Awake()
    {
        _rewardWindow.SetActive(false);
           _listPool = new QuestListPool();
        _listPool.Init("Prefab/Quest List UI Element", _listElementContainer.transform, 0);
        _questWindow.SetActive(true);
    }

    public void AddQuestListUI(QuestInfo info) 
    {
        var ui = _listPool.Set();
        if (info.QuestType == 1) ui.Instance.transform.SetAsFirstSibling();
        if (info.GOAL_TYPE == QuestInfo.QUEST_EVENT.MOVE_TO) 
        {
            UIManager.instance.gameSystemUI.pinUI.SetPinUI(new Vector3Int(info.GoalArg[0], info.GoalArg[1], info.GoalArg[2]));
        }
        ui.Instance.GetComponent<QuestListElement>().SetQuestListElement(info);
        Debug.Log(ui);

    }
    public void DeleteQuestListUI(int idx) 
    {
        QuestListElement listElement = _listPool.Find(idx);
        if (listElement == null) 
        {
            Debug.Log("해당 인덱스의 퀘스트가 활성화되어있지 않습니다. 인덱스: " + idx);
            return;
        }

        UIManager.instance.gameSystemUI.pinUI.ClearPinUI();
        listElement.CompleteQuestUI(out string rewardText);
        StopAllCoroutines();
        StartCoroutine(ShowRewardWindow(rewardText));
    }
    IEnumerator ShowRewardWindow(string rewardText) 
    {
        _rewardText.GetComponent<TextMeshProUGUI>().text = rewardText;
        _rewardWindow.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        _rewardWindow.SetActive(false);
        yield break;
    }

    public void ClickQuestButton()
    {
        _questWindow.SetActive(!_questWindow.activeSelf);
    }
    public void OpenItemTooltip(int index, Vector3 pos) 
    {
        pos = ScreenOverCorrector.GetCorrectedUIPosition(GetComponent<Canvas>(), pos, _itemTooltip);
        _itemTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(GameManager.instance.itemDatabase.GetItemData(index), pos);
    }
    public void OpenSkillTooltip(int index, Vector3 pos)
    {
        pos = ScreenOverCorrector.GetCorrectedUIPosition(GetComponent<Canvas>(), pos, _skillTooltip);
        _skillTooltip.GetComponent<SkillTooltip>().SetSkillTooltip(index, pos);
    }
    public override void ClosePopupWindow()
    {
        _itemTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        _skillTooltip.GetComponent<SkillTooltip>().CloseUI();
        base.ClosePopupWindow();
    }
}
