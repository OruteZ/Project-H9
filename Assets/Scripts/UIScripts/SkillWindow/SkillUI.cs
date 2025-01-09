using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬의 습득 상태를 정의합니다.
/// NotLearnable = 배울 수 없음.
/// Learnable = 배울 수 있음.
/// Learned = 배움. (더 이상 못 배움)
/// </summary>
public enum LearnStatus
{
    NotLearnable,
    Learnable,
    Learned
};

/// <summary>
/// 스킬 트리 및 스킬 습득 등을 관리하는 스킬창 UI 전반에 대한 기능을 수행하는 클래스
/// </summary>
public class SkillUI : UISystem
{

    private SkillManager _skillManager;
    private int _currentSkillIndex;

    private List<int> _currentKeywordIndex = new List<int>();

    [Header("Skill UIs")]
    [SerializeField] private GameObject _skillWindow;//?
    [SerializeField] private GameObject _skillUIButtons;
    [SerializeField] private GameObject _skillTooltipWindow;
    [SerializeField] private GameObject _skillPointText;

    [SerializeField] private GameObject _skillTree;
    [SerializeField] private GameObject _skillCharacterSection;
    [SerializeField] private GameObject _skillRevoloverSection;
    [SerializeField] private GameObject _skillRepeaterSection;
    [SerializeField] private GameObject _skillShotgunSection;

    [SerializeField] private GameObject _skillCharacterSectionButton;
    [SerializeField] private GameObject _skillRevoloverSectionButton;
    [SerializeField] private GameObject _skillRepeaterSectionButton;
    [SerializeField] private GameObject _skillShotgunSectionButton;
    public enum SkillSection 
    {
        Character,
        Revolover,
        Repeater,
        Shotgun
    }
    private SkillSection skillSection = SkillSection.Character;
    private List<GameObject> _skillRootNodes;

    [SerializeField] private GameObject _skillTreeScroll;
    private bool _isScrollInit;
    public SkillTooltip skillTooltip { get; private set; }
    private void Awake()
    {
        _skillCharacterSection.SetActive(true);
        _skillRevoloverSection.SetActive(true);
        _skillRepeaterSection.SetActive(true);
        _skillShotgunSection.SetActive(true);
        _skillCharacterSection.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        _skillRevoloverSection.GetComponent<RectTransform>().localPosition = new Vector3(_skillRevoloverSection.GetComponent<RectTransform>().sizeDelta.x, 0, 0);
        _skillRepeaterSection.GetComponent<RectTransform>().localPosition = new Vector3(_skillRepeaterSection.GetComponent<RectTransform>().sizeDelta.x, 0, 0);
        _skillShotgunSection.GetComponent<RectTransform>().localPosition = new Vector3(_skillShotgunSection.GetComponent<RectTransform>().sizeDelta.x, 0, 0);

        _skillRootNodes = new();
        for (int i = 0; i < _skillCharacterSection.transform.GetChild(1).childCount; i++)
        {
            if (_skillCharacterSection.transform.GetChild(1).GetChild(i).GetComponent<SkillTreeElement>().isRootNode)
            {
                _skillRootNodes.Add(_skillCharacterSection.transform.GetChild(1).GetChild(i).gameObject);
            }
        }

        _skillTreeScroll.GetComponent<Scrollbar>().value = 1;
    }
    void Start()
    {
        _skillManager = SkillManager.instance;
        skillTooltip = _skillTooltipWindow.GetComponent<SkillTooltip>();
        //GetComponent<Image>().sprite = null;
        _isScrollInit = false;
        UpdateAllSkillUINode();

        _skillCharacterSectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[16];
        _skillRevoloverSectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[17];
        _skillRepeaterSectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[18];
        _skillShotgunSectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[19];

        SetSkillSection(0);
    }
    private void Update()
    {
        while (!_isScrollInit && _skillTreeScroll.GetComponent<Scrollbar>().value != 1)
        {
            _skillTreeScroll.GetComponent<Scrollbar>().value = 1;
            if (_skillTreeScroll.GetComponent<Scrollbar>().value == 1) _isScrollInit = true;
        }
        if (Input.GetKeyDown(KeyCode.F2)) 
        {
            _skillManager.AddSkillPoint(1);
            UpdateSkillPointUI();
        }
    }
    public override void OpenUI()
    {
        _isScrollInit = false;
        UpdateAllSkillUINode();
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        CloseSkillTooltip();
    }
    public override void OpenPopupWindow()
    {
        UIManager.instance.SetUILayer(3);
        _skillTooltipWindow.SetActive(true);
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.SetUILayer(2);
        CloseSkillTooltip();
    }

    /// <summary>
    /// 각 SkillTreeElement들이 클릭되었을 때 실행됩니다.
    /// 스킬 툴팁창을 표시하는 기능들을 호출합니다.
    /// </summary>
    /// <param name="pos"> 클릭된 UI요소의 위치 </param>
    /// <param name="btnIndex"> 클릭된 skill의 고유번호 </param>
    public void ClickSkillUIButton(int btnIndex, Vector3 pos)
    {
        _currentKeywordIndex.Clear();
        pos.x += 10 * UIManager.instance.GetCanvasScale();
        pos.y -= 10 * UIManager.instance.GetCanvasScale();
        _skillTooltipWindow.GetComponent<SkillTooltip>().SetSkillTooltip(btnIndex, pos);

        OpenPopupWindow();
    }
    /// <summary>
    /// 스킬 툴팁창을 닫습니다.
    /// 스킬 툴팁창 외부를 클릭하거나, 스킬 툴팁창의 닫기 버튼을 클릭하거나, 스킬창 전체가 닫히면 실행됩니다.
    /// </summary>
    public void CloseSkillTooltip()
    {
        _skillTooltipWindow.GetComponent<SkillTooltip>().CloseUI();
    }

    public void UpdateAllSkillUINode()
    {
        UpdateSkillPointUI();
        GameObject[] selections =
            {
                _skillCharacterSection,
                _skillRevoloverSection,
                _skillRepeaterSection,
                _skillShotgunSection
            };
        for (int j = 0; j < selections.Length; j++)
        {
            Transform buttons = selections[j].transform.GetChild(1);
            for (int i = 0; i < buttons.childCount; i++)
            {
                UpdateSkillUINode(buttons.GetChild(i).gameObject);
            }
        }
    }
    public void UpdateRelatedSkillNodes(int index)
    {
        UpdateSkillPointUI();
        GameObject targetNode = FindSkillNode(index);
        UpdateSkillUINode(targetNode);

        List<GameObject> arrows = targetNode.GetComponent<SkillTreeElement>().GetPostArrow();
        foreach (GameObject arrow in arrows)
        {
            UpdateSkillUINode(arrow.GetComponent<SkillTreeArrow>().GetPostSkillNode());
        }
    }
    private void UpdateSkillUINode(GameObject ui)
    {
        if (ui == null) return;
        SkillTreeElement _skillElement = ui.GetComponent<SkillTreeElement>();
        Skill _skill = _skillManager.GetSkill(_skillElement.GetSkillUIIndex());
        if (_skill == null) return;

        LearnStatus state = LearnStatus.NotLearnable;
        if (_skill.isLearned)
        {
            state = LearnStatus.Learned;
        }
        if (_skill.isLearnable && GameManager.instance.CompareState(GameState.WORLD))
        {
            state = LearnStatus.Learnable;
        }
        _skillElement.SetSkillButtonEffect(state);

        if (_skill.skillLevel > 0)
        {
            _skillElement.SetSkillArrow();
        }
    }
    private GameObject FindSkillNode(int index) 
    {
        foreach (GameObject root in _skillRootNodes) 
        {
            GameObject findResult = root.GetComponent<SkillTreeElement>().rFindSkillNodeAtChildren(index);
            if (findResult != null) 
            {
                return findResult;
            }
        }
        return null;
    }

    private void UpdateSkillPointUI()
    {
        _skillPointText.GetComponent<TextMeshProUGUI>().text = "Skill Point: " + _skillManager.GetSkillPoint().ToString();
    }

    public void OnCloseBtnClick()
    {
        UIManager.instance.SetCharacterCanvasState(false);
        UIManager.instance.SetSkillCanvasState(false);
        UIManager.instance.SetPauseMenuCanvasState(false);
    }

    public void SetKeywordTooltipContents(List<int> keywords) 
    {
        if (keywords is null) return;
        foreach (int i in keywords)
        {
            if (_currentKeywordIndex.Contains(i)) continue;
            _currentKeywordIndex.Add(i);
            KeywordScript kw = _skillManager.GetSkillKeyword(i);
            _skillTooltipWindow.GetComponent<SkillTooltip>().SetKeywordTooltipContents(kw);
        }
    }

    public void SetSkillSection(int s)
    {
        SkillSection section = (SkillSection)s;
        skillSection = section;
        GameObject[] contents =
        {
            _skillCharacterSection,
            _skillRevoloverSection,
            _skillRepeaterSection,
            _skillShotgunSection
        };
        GameObject[] buttons =
        {
            _skillCharacterSectionButton,
            _skillRevoloverSectionButton,
            _skillRepeaterSectionButton,
            _skillShotgunSectionButton
        };

        for (int i = 0; i < contents.Length; i++)
        {
            contents[i].GetComponent<RectTransform>().localPosition = new Vector3(contents[i].GetComponent<RectTransform>().sizeDelta.x, 0, 0);
            ColorBlock b = buttons[i].GetComponent<Button>().colors;
            b.normalColor = buttons[i].GetComponent<Button>().colors.pressedColor;
            buttons[i].GetComponent<Button>().colors = b;
        }
        contents[(int)section].GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        ColorBlock block = buttons[(int)section].GetComponent<Button>().colors;
        block.normalColor = buttons[(int)section].GetComponent<Button>().colors.selectedColor;
        buttons[(int)section].GetComponent<Button>().colors = block;

        _skillTree.GetComponent<ScrollRect>().content = contents[(int)section].GetComponent<RectTransform>();
        _skillRootNodes.Clear();

        _skillUIButtons = contents[(int)section].transform.GetChild(1).gameObject;
        for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
        {
            if (_skillUIButtons.transform.GetChild(i).GetComponent<SkillTreeElement>().isRootNode) 
            {
                _skillRootNodes.Add(contents[(int)section].transform.GetChild(1).GetChild(i).gameObject);
            }
        }
        UpdateAllSkillUINode();
    }
}
