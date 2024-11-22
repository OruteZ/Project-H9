using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetListUI : UISystem
{
    [SerializeField] private GameObject _targetListWindow;
    [SerializeField] private GameObject _targetListContent;
    [SerializeField] private GameObject _targetQuestListContent;

    private GameObject _prevSelectedObject = null;
    private void Awake()
    {
        for (int i = 0; i < _targetQuestListContent.transform.childCount; i++)
        {
            _targetQuestListContent.transform.GetChild(i).GetComponent<TargetQuestListElement>().CloseUI();
        }
        CloseUI();
    }
    private void Update()
    {
        if (!_targetListWindow.activeSelf) return;
        if (_prevSelectedObject != EventSystem.current.currentSelectedGameObject) 
        {
            _prevSelectedObject = EventSystem.current.currentSelectedGameObject;
            OpenTargetUI();
        }
    }
    public void OnClickTargetListButton() 
    {
        if (_targetListWindow.activeSelf)
        {
            UIManager.instance.SetUILayer(1);
            CloseUI();
        }
        else 
        {
            OpenTargetUI();
            OpenTargetQuestList(FindCurrentProgressingQuestsTarget().GetTargetQuestList);
        }
    }
    private void OpenTargetUI() 
    {
        for (int i = 0; i < _targetListContent.transform.childCount; i++)
        {
            _targetListContent.transform.GetChild(i).GetComponent<TargetListUIElement>().SetTargetListUIElement(this);
        }
        _targetListWindow.SetActive(true);
        OpenUI();
    }
    private TargetListUIElement FindCurrentProgressingQuestsTarget()
    {
        QuestInfo CurrentMainQuestInfo = null;
        foreach (var qi in UIManager.instance.gameSystemUI.questUI.GetCurrentProgressingQuests) 
        {
            if (qi.QuestType == 1) 
            {
                CurrentMainQuestInfo = qi;
                break;
            }
        }
        if (CurrentMainQuestInfo == null) return null;

        for (int i = 0; i < _targetListContent.transform.childCount; i++)
        {
            int[] targetList = _targetListContent.transform.GetChild(i).GetComponent<TargetListUIElement>().GetTargetQuestList;
            foreach (int index in targetList) 
            {
                if (index == CurrentMainQuestInfo.Index) 
                {
                    return _targetListContent.transform.GetChild(i).GetComponent<TargetListUIElement>();
                }
            }
        }
        return null;
    }
    public override void CloseUI()
    {
        _targetListWindow.SetActive(false);
    }
    public void OpenTargetQuestList(int[] targetQuestList)
    {
        if (targetQuestList == null) return;
        if (_targetQuestListContent.transform.childCount < targetQuestList.Length) 
        {
            Debug.LogError("Not enough ui elements");
            return;
        }

        for (int i = 0; i < _targetQuestListContent.transform.childCount; i++) 
        {
            if (i < targetQuestList.Length)
            {
                QuestInfo qInfo = GameManager.instance.GetQuestInfo(targetQuestList[i]);
                _targetQuestListContent.transform.GetChild(i).GetComponent<TargetQuestListElement>().SetQuestListElement(qInfo, out string str);
            }
            else
            {
                _targetQuestListContent.transform.GetChild(i).GetComponent<TargetQuestListElement>().CloseUI();
            }
        }
    }
}
