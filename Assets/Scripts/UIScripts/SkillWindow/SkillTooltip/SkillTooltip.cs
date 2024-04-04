using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SkillTooltip : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _skillTooltip;
    [SerializeField] private GameObject _skillTooltipNameText;
    [SerializeField] private GameObject _skillTooltipDescriptionText;
    [SerializeField] private GameObject _skillTooltipButtonText;
    [SerializeField] private GameObject _skillKeywordTooltipContainer;
    [SerializeField] private GameObject _skillCostUI;
    [SerializeField] private GameObject _skillSectionIcon;
    [SerializeField] private GameObject _skillSectionText;

    private SkillManager _skillManager;
    private GameObject _currentSelectedUI = null;
    private int _currentSkillIndex;
    private bool _isInteractableButton;

    static private SkillKeywordPool _keywordTooltips = null;
    private List<SkillKeywordWrapper> _activeKeywordTooltips = new List<SkillKeywordWrapper>();

    // Start is called before the first frame update
    void Start()
    {
        _skillManager = SkillManager.instance;
        _currentSkillIndex = 0;
        _isInteractableButton = false;

        if (_keywordTooltips == null)
        {
            _keywordTooltips = new SkillKeywordPool();
            _keywordTooltips.Init("Prefab/Keyword Tooltip", _skillKeywordTooltipContainer.transform, 0);
        }
    }
    private void Update()
    {
        if (Input.mouseScrollDelta != Vector2.zero) 
        {
            CloseUI();
        }

        if (!gameObject.activeInHierarchy) return;
        if (!_skillKeywordTooltipContainer.activeInHierarchy) return;
        if (_skillKeywordTooltipContainer.transform.childCount == 0) return;

        GameObject rootGameObject = _skillKeywordTooltipContainer.transform.GetChild(0).gameObject;
        if (rootGameObject is null) return;
        rootGameObject.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        rootGameObject.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }

    public void SetSkillTooltip(GameObject ui)
    {
        OpenUI();
        _currentSelectedUI = ui;
        int skillIndex = ui.GetComponent<SkillTreeElement>().GetSkillUIIndex();
        if (_currentSkillIndex != skillIndex) 
        {
            _currentSkillIndex = skillIndex;
        }
        _isInteractableButton = false;
        GetComponent<RectTransform>().position = _currentSelectedUI.GetComponent<RectTransform>().position;
        //GetComponent<RectTransform>().position = Input.mousePosition;
        Skill currentSkill = _skillManager.GetSkill(skillIndex);
        if (currentSkill == null) return;

        _skillTooltipNameText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillName(_currentSkillIndex);
        _skillTooltipDescriptionText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillDescription(_currentSkillIndex, out var keywords);
        _skillCostUI.SetActive(!currentSkill.skillInfo.IsPassive());
        if (!currentSkill.skillInfo.IsPassive())
        {
            ActiveInfo aInfo = _skillManager.activeDB.GetActiveInfo(currentSkill.skillInfo.index);
            _skillCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(aInfo.cost, aInfo.ammoCost, true);    //ammo cost 지울 수도 있음.
        }
        ItemType sectionType = ItemType.Etc + currentSkill.skillInfo.section;
        bool isThereSectionCondition = (ItemType.Revolver <= sectionType && sectionType <= ItemType.Shotgun);
        _skillSectionIcon.SetActive(isThereSectionCondition);
        _skillSectionText.SetActive(isThereSectionCondition);
        if (isThereSectionCondition)
        {
            _skillSectionIcon.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo(sectionType.ToString());
            _skillSectionText.GetComponent<TextMeshProUGUI>().text = sectionType.ToString();
        }

        UIManager.instance.skillUI.SetKeywordTooltipContents(keywords);
        _skillTooltipDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        _skillTooltip.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        TextMeshProUGUI buttonText = _skillTooltipButtonText.GetComponent<TextMeshProUGUI>();
        if (currentSkill.isLearnable)
        {
            if (_skillManager.IsEnoughSkillPoint())
            {
                if (GameManager.instance.CompareState(GameState.World))
                {
                    buttonText.text = "습득";
                    _isInteractableButton = true;
                }
                else
                {
                    buttonText.text = "전투 중 스킬 습득 불가";
                }
            }
            else
            {
                buttonText.text = "스킬 포인트 부족";
            }
        }
        else
        {
            if (currentSkill.isLearned)
            {
                buttonText.text = "습득 완료";
            }
            else
            {
                buttonText.text = "습득 불가";
            }
        }
    }

    /// <summary>
    /// 해당 스킬을 습득하는 것이 가능하다면 SkillManager를 통해 스킬을 습득합니다.
    /// 스킬 툴팁창의 스킬습득버튼을 클릭할 시 실행됩니다.
    /// </summary>
    public void ClickLearnSkill()
    {
        if (_isInteractableButton && _skillManager.LearnSkill(_currentSkillIndex))
        {
            UIManager.instance.skillUI.UpdateRelatedSkillNodes(_currentSkillIndex);
        }
        SetSkillTooltip(_currentSelectedUI);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _skillKeywordTooltipContainer.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _skillKeywordTooltipContainer.SetActive(false);
        //CloseUI();
    }
    public void SetKeywordTooltipContents(KeywordScript keyword)
    {
        if (keyword == null) return;
        var t = _keywordTooltips.Set();
        t.tooltip.SetSkillKeywordTooltip(keyword.name, keyword.GetDescription(), _activeKeywordTooltips.Count);
        _activeKeywordTooltips.Add(t);
    }
    public void ClearKeywordTooltips()
    {
        _keywordTooltips.Reset();
        _activeKeywordTooltips.Clear();
        _skillKeywordTooltipContainer.SetActive(false);
    }
    public override void CloseUI()
    {
        ClearKeywordTooltips();
        base.CloseUI();
    }
}
