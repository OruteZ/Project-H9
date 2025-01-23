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
    [SerializeField] private GameObject _skillTooltipButton;
    [SerializeField] private GameObject _skillTooltipButtonText;
    [SerializeField] private GameObject _skillKeywordTooltipContainer;
    [SerializeField] private GameObject _skillCostUI;
    [SerializeField] private GameObject _skillSectionIcon;
    [SerializeField] private GameObject _skillSectionText;

    private SkillManager _skillManager;
    private int _currentSkillIndex;
    private bool _isButtonInteractable;

    private SkillKeywordPool _keywordTooltips = null;
    private List<SkillKeywordWrapper> _activeKeywordTooltips = new List<SkillKeywordWrapper>();

    // Start is called before the first frame update
    void Start()
    {
        _skillManager = SkillManager.instance;
        _currentSkillIndex = 0;
        _isButtonInteractable = false;

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

    public void SetSkillTooltip(int index, Vector3 pos)
    {
        OpenUI();
        _currentSkillIndex = index;
        _isButtonInteractable = false;
        GetComponent<RectTransform>().position = pos;
        //GetComponent<RectTransform>().position = Input.mousePosition;
        _skillManager = SkillManager.instance;
        Skill currentSkill = _skillManager.GetSkill(index);
        if (currentSkill == null) return;

        _skillTooltipNameText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillName(_currentSkillIndex);
        _skillTooltipDescriptionText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillDescription(_currentSkillIndex, out var keywords);
        _skillCostUI.SetActive(!currentSkill.skillInfo.IsPassive());
        if (!currentSkill.skillInfo.IsPassive())
        {
            ActiveInfo aInfo = _skillManager.activeDB.GetActiveInfo(currentSkill.skillInfo.index);
            _skillCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(aInfo.cost, aInfo.ammoCost, true);    //ammo cost ���� ���� ����.
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

        if (gameObject.tag != "SkillUI")
        {
            _skillTooltipButton.SetActive(false);
            return;
        }
        _skillTooltipButton.SetActive(true);
        TextMeshProUGUI buttonText = _skillTooltipButtonText.GetComponent<TextMeshProUGUI>();
        if (currentSkill.isLearnable)
        {
            if (_skillManager.IsEnoughSkillPoint())
            {
                if (GameManager.instance.CompareState(GameState.WORLD))
                {
                    buttonText.text = UIManager.instance.UILocalization[34];
                    _isButtonInteractable = true;
                }
                else
                {
                    buttonText.text = UIManager.instance.UILocalization[38];
                }
            }
            else
            {
                buttonText.text = UIManager.instance.UILocalization[37];
            }
        }
        else
        {
            if (currentSkill.isLearned)
            {
                buttonText.text = UIManager.instance.UILocalization[36];
            }
            else
            {
                buttonText.text = UIManager.instance.UILocalization[35];
            }
        }
    }

    /// <summary>
    /// �ش� ��ų�� �����ϴ� ���� �����ϴٸ� SkillManager�� ���� ��ų�� �����մϴ�.
    /// ��ų ����â�� ��ų�����ư�� Ŭ���� �� ����˴ϴ�.
    /// </summary>
    public void ClickLearnSkill()
    {
        if (_isButtonInteractable && _skillManager.LearnSkill(_currentSkillIndex))
        {
            UIManager.instance.skillUI.UpdateRelatedSkillNodes(_currentSkillIndex);
            SoundManager.instance.PlaySFX("UI_LearnSkill");
        }
        else
        {
            SoundManager.instance.PlaySFX("UI_Denied");
        }
        SetSkillTooltip(_currentSkillIndex, GetComponent<RectTransform>().position);
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
