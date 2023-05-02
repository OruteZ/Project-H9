using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{

    [SerializeField] private Canvas WorldCanvas;
    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private Canvas CharacterCanvas;
    [SerializeField] private Canvas SkillCanvas;
    [SerializeField] private Canvas OptionCanvas;

    [SerializeField] private SkillManager _SkillManager;
    [SerializeField] private GameObject SkillWindow;
    [SerializeField] private GameObject SkillTooltipWindow;
    [SerializeField] private GameObject SkillPointText;
    private int currentSkillIndex;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnOffCharacterWindow() 
    {
        CharacterCanvas.enabled = !CharacterCanvas.enabled;

        if (CharacterCanvas.enabled)
        {
            CloseSkillCanvas();
            OptionCanvas.enabled = false;
        }
    }
    public void OnOffSkillWindow()
    {
        SkillCanvas.enabled = !SkillCanvas.enabled;
        UpdateSkillUiImage();

        if (SkillCanvas.enabled)
        {
            CharacterCanvas.enabled = false;
            OptionCanvas.enabled = false;
        }
    }
    public void OnOffOptionWindow()
    {
        OptionCanvas.enabled = !OptionCanvas.enabled;

        if (OptionCanvas.enabled)
        {
            CharacterCanvas.enabled = false; 
            CloseSkillCanvas();
        }
    }

    private void CloseSkillCanvas()
    {
        SkillCanvas.enabled = false;
        CloseSkillTooltip();
    }

    public void ClickSkillUiButton(Transform _transform, int btnIndex) 
    {
        SkillTooltipWindow.transform.position = _transform.position;
        SetTooltipWindow(btnIndex);
        SkillTooltipWindow.SetActive(true);
    }
    private void SetTooltipWindow(int index) 
    {
        Skill currentSkill = _SkillManager.GetSkill(index);
        currentSkillIndex = index;
        SkillTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.name;
        SkillTooltipWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.description;

        TextMeshProUGUI buttonText = SkillTooltipWindow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (currentSkill.isLearnable)
        {
            if (_SkillManager.IsEnoughSkillPoint())
            {
                buttonText.text = "습득";
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
    public void CloseSkillTooltip()
    {
        SkillTooltipWindow.SetActive(false);
    }
    public void ClickLearnSkill() 
    {
        Debug.Log(currentSkillIndex);
        if (_SkillManager.LearnSkill(currentSkillIndex))
        {
            UpdateSkillUiImage();
        }
        SetTooltipWindow(currentSkillIndex);
    }

    enum LearnState 
    {
        NOT_LEARNED,
        LEARNABLE,
        ALREADLY_LEARNED
    };
    private void UpdateSkillUiImage() 
    {
        UpdateSkillPointUi();
        for (int i = 0; i < SkillWindow.transform.childCount - 3; i++) 
        {
            SkillUI _skillUi = SkillWindow.transform.GetChild(i + 1).GetComponent<SkillUI>();
            Skill _skill = _SkillManager.GetSkill(_skillUi.GetIndex());

            LearnState state = LearnState.NOT_LEARNED;
            if (_skill.isLearned) 
            {
                state = LearnState.ALREADLY_LEARNED;
            }
            if (_skill.isLearnable)
            {
                state = LearnState.LEARNABLE;
            }
            _skillUi.SetSkillButtonEffect((int)state);
        }
    }
    private void UpdateSkillPointUi() 
    {
        SkillPointText.GetComponent<TextMeshProUGUI>().text = "SP: " + _SkillManager.GetSkillPoint().ToString();
    }
}
