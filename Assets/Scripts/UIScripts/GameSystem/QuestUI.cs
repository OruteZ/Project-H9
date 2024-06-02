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
    [SerializeField] private GameObject _popupWindow;
    [SerializeField] private GameObject _popupText;

    public WorldCamera worldCamera;

    private QuestListPool _listPool = null;

    void Awake()
    {
        _popupWindow.SetActive(false);
        _listPool = new QuestListPool();
        _listPool.Init("Prefab/Quest List UI Element", _listElementContainer.transform, 0);
        _questWindow.SetActive(true);
    }

    public void AddQuestListUI(QuestInfo info) 
    {
        var ui = _listPool.Set();
        Debug.Log($"info Pin null: {info.Pin == null}");
        Debug.Log($"info Pin len: {info.Pin.Length}");
        if (info.Pin.Length > 0) 
        {
            Vector3Int pinPos = new Vector3Int(info.Pin[0], info.Pin[1], info.Pin[2]);
            UIManager.instance.gameSystemUI.pinUI.SetPinUI(pinPos);
            Tile target = FieldSystem.tileSystem.GetTile(pinPos);
            if (target is null)
            {
                Debug.LogError("해당 Hex 좌표의 Tile은 존재하지 않습니다. Hex 좌표: " + pinPos);
                return;
            }
            Vector3 _targetPos = target.gameObject.transform.position;
            worldCamera.SetPosition(_targetPos);
        }
        ui.Instance.GetComponent<QuestListElement>().SetQuestListElement(info, out string popupText);
        _listPool.Sort();
        StartCoroutine(StartQuestUI(popupText));

    }
    public void DeleteQuestListUI(QuestInfo info) 
    {
        QuestListElement listElement = _listPool.Find(info.Index);
        if (listElement == null) 
        {
            Debug.Log("해당 인덱스의 퀘스트가 활성화되어있지 않습니다. 인덱스: " + info.Index);
            return;
        }

        _listPool.Sort();

        listElement.CompleteQuestUI(out string popupText);
        StartCoroutine(EndQuestUI(popupText));
    }
    IEnumerator StartQuestUI(string popupText)
    {
        while (true)
        {
            _popupText.GetComponent<TextMeshProUGUI>().text = popupText;
            _popupWindow.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            _popupWindow.SetActive(false);
            yield break;
        }
    }
    IEnumerator EndQuestUI(string popupText) 
    {
        while (true)
        {
            _popupText.GetComponent<TextMeshProUGUI>().text = popupText;
            _popupWindow.SetActive(true);
            Player player = FieldSystem.unitSystem.GetPlayer();
            if (player == null)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }
            var actions = player.GetUnitActionArray();
            foreach (var a in actions)
            {
                if (a is IdleAction)
                {
                    player.SelectAction(a);
                }
            }

            yield return new WaitForSeconds(1.5f);
            _popupWindow.SetActive(false);
            yield return new WaitForSeconds(.5f);
            UIManager.instance.gameSystemUI.conversationUI.StartNextConversation();

            player = FieldSystem.unitSystem.GetPlayer();
            if (player == null) 
            {
                yield break;
            } 
            foreach (var a in actions)
            {
                if (a is MoveAction)
                {
                    player.SelectAction(a);
                }
            }
            
            
            yield break;
        }
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
        if(_itemTooltip.activeSelf) _itemTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        if (_skillTooltip.activeSelf) _skillTooltip.GetComponent<SkillTooltip>().CloseUI();
        base.ClosePopupWindow();
    }
}
