using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OptionUI : UISystem
{
    [SerializeField] private GameObject _optionWindow;

    [SerializeField] private GameObject _mainVolumeBar;
    [SerializeField] private GameObject _BGMVolumeBar;
    [SerializeField] private GameObject _SFXVolumeBar;

    [SerializeField] private GameObject _fullScreenToggle;
    [SerializeField] private GameObject _resolutionDropDown;

    [SerializeField] private GameObject _lauguageDropDown;
    [SerializeField] private GameObject _lauguageChangePopup;
    private ScriptLanguage prevLanguage;
    public bool isOpened;


    private OptionSetting _optionSetting;

    private List<Vector2Int> resolutions;


    // Start is called before the first frame update
    void Awake()
    {
        isOpened = false;
        _optionWindow.SetActive(false);
        GameManager.instance.initialOptionSetting.resolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        GameManager.instance.initialOptionSetting.isFullScreen = Screen.fullScreen;

        _optionSetting = GameManager.instance.initialOptionSetting;

    }
    private void Start()
    {
        LoadOption(GameManager.instance.user.optionSetting);
        SetOptionWindow();
    }
    public OptionSetting GetOptionSetting()
    {
        return _optionSetting;
    }
    public void LoadOption(OptionSetting setting) 
    {
        if (setting.lauguage == ScriptLanguage.NULL) return;
        _optionSetting = setting;
        SetOptionWindow();
    }
    public override void OpenUI()
    {
        if (_optionWindow.activeSelf) 
        {
            CloseUI();
            return;
        }
        base.OpenUI();
        SetOptionWindow();
        _optionWindow.SetActive(true);

        _lauguageChangePopup.SetActive(false);
        prevLanguage = _optionSetting.lauguage;
        isOpened = true;
    }
    public override void CloseUI()
    {
        if (_optionSetting.lauguage != prevLanguage)
        {
            _lauguageChangePopup.SetActive(true);
        }
        else
        {
            GameManager.instance.Save();
            _optionWindow.SetActive(false);
            base.CloseUI();
            isOpened = false;
        }
    }
    public void OnClickChangeLanguageBtn() 
    {
        UserAccount.Language = _optionSetting.lauguage;
        prevLanguage = _optionSetting.lauguage;
        GameManager.instance.Save();
        UIManager.instance.pauseMenuUI.BackToTitle();
    }
    public void OnClickCancelLanguageBtn() 
    {
        _optionSetting.lauguage = prevLanguage;
        CloseUI();
    }

    public void SetOptionWindow()
    {
        _mainVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.MainVolume;
        _BGMVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.BGMVolume;
        _SFXVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.SFXVolume;

        _fullScreenToggle.GetComponent<Toggle>().isOn = _optionSetting.isFullScreen;

        resolutions = new();
        for (int i = 0; i < Screen.resolutions.Length; i++) 
        {
            if (Screen.resolutions[i].width % 10 != 0 || Screen.resolutions[i].height % 10 != 0) continue;

            resolutions.Add(new Vector2Int(Screen.resolutions[i].width, Screen.resolutions[i].height));
            
        }
        resolutions = resolutions.Distinct().ToList();

        List<string> strings = new();
        int curIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            strings.Add($"{resolutions[i].x}x{resolutions[i].y}");
            if (resolutions[i].x == _optionSetting.resolution.x &&
                resolutions[i].y == _optionSetting.resolution.y)
            {
                curIndex = i;
            }
        }
        _resolutionDropDown.GetComponent<TMP_Dropdown>().ClearOptions();
        _resolutionDropDown.GetComponent<TMP_Dropdown>().AddOptions(strings);
        _resolutionDropDown.GetComponent<TMP_Dropdown>().value = curIndex;
        _resolutionDropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();

        List<string> laugauages = new(){ "Korean", "English" };
        _lauguageDropDown.GetComponent<TMP_Dropdown>().ClearOptions();
        _lauguageDropDown.GetComponent<TMP_Dropdown>().AddOptions(laugauages);
        _lauguageDropDown.GetComponent<TMP_Dropdown>().value = (int)_optionSetting.lauguage - 1;
        _lauguageDropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();

    }

    public void ChangeVolumeOption(int index)
    {
        if (index == 0)
        {
            _optionSetting.MainVolume = _mainVolumeBar.GetComponent<Scrollbar>().value;
            SoundManager.instance.SetMainVolume(_optionSetting.MainVolume);
        }
        else if (index == 1)
        {
            _optionSetting.BGMVolume = _BGMVolumeBar.GetComponent<Scrollbar>().value;
            SoundManager.instance.SetBGMVolume(_optionSetting.BGMVolume);
        }
        if (index == 2)
        {
            _optionSetting.SFXVolume = _SFXVolumeBar.GetComponent<Scrollbar>().value;
            SoundManager.instance.SetSFXVolume(_optionSetting.SFXVolume);
        }

    }
    public void ChangeScreenOption()
    {
        if (resolutions == null) return;
        Vector2Int selectedResolution = resolutions[_resolutionDropDown.GetComponent<TMP_Dropdown>().value];
        _optionSetting.resolution = new Vector2Int(selectedResolution.x, selectedResolution.y);
        _optionSetting.isFullScreen = _fullScreenToggle.GetComponent<Toggle>().isOn;

        Screen.SetResolution(selectedResolution.x, selectedResolution.y, _optionSetting.isFullScreen);
    }
    public void ChangeLanguageOption() 
    {
        _optionSetting.lauguage = (ScriptLanguage)(_lauguageDropDown.GetComponent<TMP_Dropdown>().value + 1);
    }
    public void ResetOption()
    {
        _optionSetting = GameManager.instance.initialOptionSetting;
        SetOptionWindow();

        //???
        _mainVolumeBar.GetComponent<Scrollbar>().value = GameManager.instance.initialOptionSetting.MainVolume;
        _BGMVolumeBar.GetComponent<Scrollbar>().value = GameManager.instance.initialOptionSetting.BGMVolume;
        _SFXVolumeBar.GetComponent<Scrollbar>().value = GameManager.instance.initialOptionSetting.SFXVolume;

        List<string> laugauages = new() { "Korean", "English" };
        _lauguageDropDown.GetComponent<TMP_Dropdown>().ClearOptions();
        _lauguageDropDown.GetComponent<TMP_Dropdown>().AddOptions(laugauages);
        _lauguageDropDown.GetComponent<TMP_Dropdown>().value = (int)GameManager.instance.initialOptionSetting.lauguage - 1;
        _lauguageDropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();
    }
}

[System.Serializable]
public struct OptionSetting 
{
    public float MainVolume;
    public float BGMVolume;
    public float SFXVolume;

    public ScriptLanguage lauguage;

    public bool isFullScreen;
    public Vector2Int resolution;
}
