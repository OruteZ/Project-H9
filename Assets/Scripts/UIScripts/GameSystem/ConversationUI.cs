using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConversationUI : UISystem
{
    [SerializeField] private GameObject _conversationWindow;
    [SerializeField] private GameObject _speakerPortrait;
    [SerializeField] private GameObject _speakerText;
    [SerializeField] private GameObject _contentsText;

    public bool isConverstating { get; private set; }

    private List<(QuestInfo, bool)> _conversationQueue;

    private List<ConversationInfo> _conversationInfo;
    private List<ConversationInfo> _groupInfo;
    private int _sequenceNumber;

    private void Awake()
    {
        _conversationWindow.SetActive(false);
        isConverstating = false;

        List<List<string>> conversationTable = FileRead.Read("ConversationTable", out var column);
        if (conversationTable == null)
        {
            Debug.LogError("��ȭ ���̺��� ã�� �� �����ϴ�.");
            return;
        }
        _conversationQueue = new();
        _groupInfo = null;
        _sequenceNumber = 0;

        _conversationInfo = new List<ConversationInfo>();
        for (int i = 0; i < conversationTable.Count; i++)
        {
            int idx = int.Parse(conversationTable[i][0]);
            int g = int.Parse(conversationTable[i][1]);
            int s = int.Parse(conversationTable[i][2]);
            string n = conversationTable[i][3];
            string img = conversationTable[i][4];
            string o = conversationTable[i][4 + (int)ScriptLanguage.English];
            string t = conversationTable[i][4 + (int)UserAccount.Language];
            
            if (string.IsNullOrEmpty(t)) t = o;
            
            ConversationInfo info = new ConversationInfo(idx, g, s, n, img, o, t);
            _conversationInfo.Add(info);
        }
    }
    private List<ConversationInfo> GetConversationGroup(int g)
    {
        List<ConversationInfo> list = new List<ConversationInfo>();
        List<ConversationInfo> sortedlist = new List<ConversationInfo>();

        for (int i = 0; i < _conversationInfo.Count; i++) 
        {
            if (_conversationInfo[i].group == g) 
            {
                list.Add(_conversationInfo[i]);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].sequence == i)
            {
                sortedlist.Add(list[i]);
            }
        }

        if (sortedlist.Count == 0) sortedlist = null;
        return sortedlist;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ProgressConversation();
        }
    }

    public void PrepareToStartConversation(QuestInfo info, bool isQuestStarting)
    {
        _conversationQueue.Add((info, isQuestStarting));
        if (!isConverstating) { StartNextConversation(); }
    }
    public void ProgressConversation() 
    {
        if (_groupInfo == null) return;
        _sequenceNumber++;
        if (_groupInfo.Count <= _sequenceNumber)
        {
            EndConversation();
        }
        else
        {
            //_speakerPortrait.GetComponent<Image>().sprite = ???;
            _speakerText.GetComponent<TextMeshProUGUI>().text = _groupInfo[_sequenceNumber].speakerName;
            _contentsText.GetComponent<TextMeshProUGUI>().text = _groupInfo[_sequenceNumber].conversationText;
        }
    }
    private void EndConversation()
    {
        _groupInfo = null;
        _sequenceNumber = 0;
        _conversationWindow.SetActive(false);

        if (_conversationQueue[0].Item2)
        {
            UIManager.instance.gameSystemUI.questUI.AddQuestListUI(_conversationQueue[0].Item1);
        }
        else
        {
            // todo : �ӽ� ���� �߰��� ���� �ڵ��Դϴ�. ���� �����ؾ� �մϴ�.
            if (_conversationQueue[0].Item1 is { Index: 31 })
            {
                Debug.LogError("���� ������ �̵��մϴ�.");
                SceneManager.LoadScene("EndingScene");
            }
            UIManager.instance.gameSystemUI.questUI.DeleteQuestListUI(_conversationQueue[0].Item1);
        }
        _conversationQueue.RemoveAt(0);
        isConverstating = false;
        StopAllCoroutines();
        StartCoroutine(DelayedStartConversation());
    }
    private IEnumerator DelayedStartConversation() 
    {
        do
        {
            yield return new WaitForSeconds(2.0f);
        } while (isConverstating);

        StartNextConversation();
        yield break;
    }
    public void StartNextConversation()
    {
        if (_conversationQueue.Count <= 0) return;
        QuestInfo curquestInfo = _conversationQueue[0].Item1;

        if (_conversationQueue[0].Item2)
        {
            _groupInfo = GetConversationGroup(curquestInfo.StartConversation);
        }
        else
        {
            _groupInfo = GetConversationGroup(curquestInfo.EndConversation);
        }
        if (_groupInfo == null)
        {
            EndConversation();
            return;
        }

        _conversationWindow.SetActive(true);
        isConverstating = true;
        _sequenceNumber = 0;
        _speakerPortrait.GetComponent<Image>().sprite = _groupInfo[0].speakerImage;
        _speakerText.GetComponent<TextMeshProUGUI>().text = _groupInfo[0].speakerName;
        _contentsText.GetComponent<TextMeshProUGUI>().text = _groupInfo[0].conversationText;
    }
}

public class ConversationInfo 
{
    public int index { get; private set; }
    public int group { get; private set; }
    public int sequence { get; private set; }
    public string speakerName { get; private set; }
    public Sprite speakerImage { get; private set; }
    public string originalConversationText { get; private set; }
    public string conversationText { get; private set; }

    public ConversationInfo(int i, int g, int s, string name, string image, string originText, string text) 
    {
        index = i;
        group = g;
        sequence = s;
        speakerName = name/* + "*FixLater"*/;     //need Localization
        Texture2D texture = Resources.Load("UnitCapture/" + image) as Texture2D;
        if (texture == null) texture = Resources.Load("UnitCapture/NULL") as Texture2D;
        
        Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        speakerImage = spr;
        originalConversationText = originText;
        conversationText = text;
    } 
}
