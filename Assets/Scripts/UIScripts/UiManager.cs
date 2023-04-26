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

    [SerializeField] private SkillManager SkillManager;
    [SerializeField] private GameObject SkillTooltipWindow;
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
            SkillCanvas.enabled = false;
            OptionCanvas.enabled = false;
        }
    }
    public void OnOffSkillWindow()
    {
        SkillCanvas.enabled = !SkillCanvas.enabled;

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
            SkillCanvas.enabled = false;
        }
    }

    public void ClickSkillUiButton(GameObject _gameObject, int btnIndex) 
    {
        SkillTooltipWindow.transform.position = _gameObject.transform.position;
        SetTooltipWindow(btnIndex);
        SkillTooltipWindow.SetActive(true);
    }
    private void SetTooltipWindow(int index) 
    {
        Skill currentSkill = SkillManager.GetSkills(index);
        currentSkillIndex = index;
        SkillTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSkill.GetName();
        SkillTooltipWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentSkill.GetDescription();
        if (currentSkill.GetIsLearnable())
        {
            SkillTooltipWindow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "½Àµæ";
        }
        else
        {
            SkillTooltipWindow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "½Àµæ ºÒ°¡";
        }
    }
    public void CloseSkillTooltip()
    {
        SkillTooltipWindow.SetActive(false);
    }
}
