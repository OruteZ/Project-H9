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

    [SerializeField] private GameObject[] _skillRootNodes;

    [SerializeField] private GameObject _skillTreeScroll;
    private bool _isScrollInit;
    public SkillTooltip skillTooltip { get; private set; }
    private void Awake()
    {
        _skillTreeScroll.GetComponent<Scrollbar>().value = 1;
    }
    void Start()
    {
        _skillManager = SkillManager.instance;
        skillTooltip = _skillTooltipWindow.GetComponent<SkillTooltip>();
        //GetComponent<Image>().sprite = null;
        _isScrollInit = false;
        UpdateAllSkillUINode();
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
        for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
        {
            UpdateSkillUINode(_skillUIButtons.transform.GetChild(i).gameObject);
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
        if (ui == null || !ui.activeSelf) return;
        SkillTreeElement _skillElement = ui.GetComponent<SkillTreeElement>();
        Skill _skill = _skillManager.GetSkill(_skillElement.GetSkillUIIndex());
        if (_skill == null) return;

        LearnStatus state = LearnStatus.NotLearnable;
        if (_skill.isLearned)
        {
            state = LearnStatus.Learned;
        }
        if (_skill.isLearnable && GameManager.instance.CompareState(GameState.World))
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
}
