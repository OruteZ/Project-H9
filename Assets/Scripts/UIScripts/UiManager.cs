using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiManager : Generic.Singleton<UiManager>
{

    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;

    [SerializeField] private GameObject _backgroundButton;

    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private GameObject _skillWindow;
    [SerializeField] private GameObject _skillUIButtons;
    [SerializeField] private GameObject _skillTooltipWindow;
    [SerializeField] private GameObject _skillPointText;
    private int _currentSkillIndex;

    private ItemUIStatus _currentItemUiStatus = ItemUIStatus.Weapon;
    [SerializeField] private GameObject _weaponItemPanel;
    [SerializeField] private GameObject _usableItemPanel;
    [SerializeField] private GameObject _otherItemPanel;

    public bool isMouseOverUI;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSkillUiImage();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnOffCharacterCanvas(bool isOn)
    {
        if (isOn && _characterCanvas.enabled) isOn = false;
        _characterCanvas.enabled = isOn;

        OnOffBackgroundBtn();
    }
    public void OnOffSkillCanvas(bool isOn)
    {
        if (isOn && _skillCanvas.enabled) isOn = false;
        _skillCanvas.enabled = isOn;

        if (!_skillCanvas.enabled)
        {
            CloseSkillTooltip();
        }

        OnOffBackgroundBtn();
    }
    public void OnOffPauseMenuCanvas(bool isOn)
    {
        if (isOn && _pauseMenuCanvas.enabled) isOn = false;
        _pauseMenuCanvas.enabled = isOn;

        OnOffBackgroundBtn();
    }
    private void OnOffBackgroundBtn()
    {
        bool isActiveSomeWindow = false;
        if (_characterCanvas.enabled) isActiveSomeWindow = true;
        if (_skillCanvas.enabled) isActiveSomeWindow = true;

        _backgroundButton.SetActive(isActiveSomeWindow);
    }

    public void ClickSkillUiButton(Transform _transform, int btnIndex)
    {
        _skillTooltipWindow.transform.position = _transform.position;
        SetTooltipWindow(btnIndex);
        _skillTooltipWindow.SetActive(true);
    }
    private void SetTooltipWindow(int index)
    {
        Skill currentSkill = _skillManager.GetSkill(index);
        _currentSkillIndex = index;
        _skillTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.name;
        _skillTooltipWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.description;

        TextMeshProUGUI buttonText = _skillTooltipWindow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (currentSkill.isLearnable)
        {
            if (_skillManager.IsEnoughSkillPoint())
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
        _skillTooltipWindow.SetActive(false);
    }
    public void ClickLearnSkill()
    {
        Debug.Log(_currentSkillIndex);
        if (_skillManager.LearnSkill(_currentSkillIndex))
        {
            UpdateSkillUiImage();
        }
        SetTooltipWindow(_currentSkillIndex);
    }

    public enum LearnState
    {
        NotLearned,
        Learnable,
        AlreadyLearned
    };
    private void UpdateSkillUiImage()
    {
        UpdateSkillPointUi();
        for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
        {
            SkillUI _skillUi = _skillUIButtons.transform.GetChild(i).GetComponent<SkillUI>();
            Skill _skill = _skillManager.GetSkill(_skillUi.GetSkillUiIndex());

            LearnState state = LearnState.NotLearned;
            if (_skill.isLearned)
            {
                state = LearnState.AlreadyLearned;
            }
            if (_skill.isLearnable)
            {
                state = LearnState.Learnable;
            }
            _skillUi.SetSkillButtonEffect((int)state);

            if (_skill.skillLevel > 0)
            {
                _skillUi.SetSkillArrow();
            }
        }
    }
    private void UpdateSkillPointUi()
    {
        _skillPointText.GetComponent<TextMeshProUGUI>().text = "SP: " + _skillManager.GetSkillPoint().ToString();
    }


    private void SetCharacterStatText()
    {

    }
    private void SetWeaponStatText() 
    {

    }

    private void SetLearnedSkiilInfoUI() 
    {

    }

    public enum ItemUIStatus
    {
        Weapon,
        Usable,
        Other
    };
    public void ChangeItemUIStatus(ItemUIStatus status) 
    {
        if (_currentItemUiStatus != status) 
        {
            if (status == ItemUIStatus.Weapon)
            {
                ShowWeaponItems();
            }
            else if (status == ItemUIStatus.Usable)
            {
                ShowUsableItems();
            }
            else if (status == ItemUIStatus.Other)
            {
                ShowOtherItems();
            }
        }
    }
    private void ShowWeaponItems() 
    {
        _weaponItemPanel.GetComponent<Image>().enabled = true;
        _usableItemPanel.GetComponent<Image>().enabled = false;
        _otherItemPanel.GetComponent<Image>().enabled = false;
    }
    private void ShowUsableItems()
    {
        _weaponItemPanel.GetComponent<Image>().enabled = false;
        _usableItemPanel.GetComponent<Image>().enabled = true;
        _otherItemPanel.GetComponent<Image>().enabled = false;
    }
    private void ShowOtherItems()
    {
        _weaponItemPanel.GetComponent<Image>().enabled = false;
        _usableItemPanel.GetComponent<Image>().enabled = false;
        _otherItemPanel.GetComponent<Image>().enabled = true;
    }
}
