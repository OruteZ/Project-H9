using TMPro;
using UnityEngine;

/// <summary>
/// 스킬 트리 및 스킬 습득 등을 관리하는 스킬창 UI 전반에 대한 기능을 수행하는 클래스
/// </summary>
public class SkillUI : UISystem
{
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

    private SkillManager _skillManager;
    private int _currentSkillIndex;

    [Header("Skill UIs")]
    [SerializeField] private GameObject _skillWindow;//?
    [SerializeField] private GameObject _skillUIButtons;
    [SerializeField] private GameObject _skillTooltipWindow;
    [SerializeField] private GameObject _skillPointText;

    void Start()
    {
        _skillManager = SkillManager.instance;
        //GetComponent<Image>().sprite = ;
        UpdateSkillUIImage();
    }
    public override void OpenUI() 
    {
        base.OpenUI();
        UpdateSkillPointUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        CloseSkillTooltip();
    }
    public override void OpenPopupWindow()
    {
        UIManager.instance.previousLayer = 3;
        _skillTooltipWindow.SetActive(true);
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.previousLayer = 2;
        CloseSkillTooltip();
    }

    /// <summary>
    /// 각 SkillTreeElement들이 클릭되었을 때 실행됩니다.
    /// 스킬 툴팁창을 표시하는 기능들을 호출합니다.
    /// </summary>
    /// <param name="_transform"> 클릭된 UI요소의 위치 </param>
    /// <param name="btnIndex"> 클릭된 skill의 고유번호 </param>
    public void ClickSkillUIButton(Vector3 pos, int btnIndex)
    {
        _skillTooltipWindow.GetComponent<SkillTooltip>().SetSkillTooltip(pos, btnIndex);

        OpenPopupWindow();
    }
    /// <summary>
    /// 스킬 툴팁창을 닫습니다.
    /// 스킬 툴팁창 외부를 클릭하거나, 스킬 툴팁창의 닫기 버튼을 클릭하거나, 스킬창 전체가 닫히면 실행됩니다.
    /// </summary>
    public void CloseSkillTooltip()
    {
        _skillTooltipWindow.SetActive(false);
    }
    public void UpdateSkillUIImage()
    {
        UpdateSkillPointUI();
        for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
        {
            SkillTreeElement _skillElement = _skillUIButtons.transform.GetChild(i).GetComponent<SkillTreeElement>();
            Skill _skill = _skillManager.GetSkill(_skillElement.GetSkillUIIndex());

            LearnStatus state = LearnStatus.NotLearnable;
            if (_skill.isLearned)
            {
                state = LearnStatus.Learned;
            }
            if (_skill.isLearnable)
            {
                state = LearnStatus.Learnable;
            }
            _skillElement.SetSkillButtonEffect((int)state);

            if (_skill.skillLevel > 0)
            {
                _skillElement.SetSkillArrow();
            }
        }
    }
    private void UpdateSkillPointUI()
    {
        _skillPointText.GetComponent<TextMeshProUGUI>().text = "SP: " + _skillManager.GetSkillPoint().ToString();
    }

    public void OnCloseBtnClick()
    {
        UIManager.instance.SetCharacterCanvasState(false);
        UIManager.instance.SetSkillCanvasState(false);
        UIManager.instance.SetPauseMenuCanvasState(false);
    }
}
