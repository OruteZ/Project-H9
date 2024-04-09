using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 수주한 퀘스트를 표시하는 기능을 구현한 클래스
/// </summary>
public class QuestUI : UISystem
{
    [SerializeField] private GameObject _questWindow;
    [SerializeField] private GameObject _listElementContainer;
    [SerializeField] private GameObject _itemTooltip;
    [SerializeField] private GameObject _skillTooltip;

    private QuestListPool _listPool = null;

    void Start()
    {
        _questWindow.SetActive(true);
    }
    public void SetQuestListUI()
    {
        //var quests = getQuests()

        //_listPool.Reset();

        //var mainQuestUI = _listPool.Set();
        //var mainQuest = null;
        //for (int i = 0; i < quests.Count; i++) 
        //{
        //    if (quests[i].isMainQuest) 
        //    {
        //        mainQuest = quests[i];
        //        continue;
        //    }
        //    var ui = _listPool.Set();
        //    ui.Instance.GetComponent<QuestListElement>().SetQuestListElement(quests[i]);
        //}
        //if (mainQuest == null) 
        //{
        //    Debug.LogError("메인 퀘스트가 존재하지 않아 UI에 표시할 수 없습니다.");
        //    return;
        //}
        //mainQuestUI.Instance.GetComponent<QuestListElement>().SetQuestListElement(mainQuest);
    }

    public void AddQuestListUI() 
    {

    }
    public void DeleteQuestListUI(int idx) 
    {
        QuestListElement listElement = _listPool.Find(idx);
        if (listElement == null) 
        {
            Debug.Log("해당 인덱스의 퀘스트가 활성화되어있지 않습니다. 인덱스: " + idx);
            return;
        }

        listElement.CompleteQuestUI();
    }

    public void ClickQuestButton()
    {
        _questWindow.SetActive(!_questWindow.activeSelf);
    }
}
