using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OptionUI : UISystem
{
    [SerializeField] private GameObject _optionWindow;

    [SerializeField] private OptionSetting _initialOptionSetting;

    [SerializeField] private GameObject _mainVolumeBar;
    [SerializeField] private GameObject _BGMVolumeBar;
    [SerializeField] private GameObject _SFXVolumeBar;

    [SerializeField] private GameObject _fullScreenToggle;
    [SerializeField] private GameObject _resolutionDropDown;

    [SerializeField] private GameObject _lauguageDropDown;

    private OptionSetting _optionSetting;

    private List<Vector2Int> resolutions;

    // Start is called before the first frame update
    void Awake()
    {
        _optionWindow.SetActive(false);
        _initialOptionSetting.resolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        _initialOptionSetting.isFullScreen = Screen.fullScreen;

        //need loading userdata
        _optionSetting = _initialOptionSetting;

        SetOptionWindow();
    }
    public void LoadOption(OptionSetting setting) 
    {
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
    }
    public override void CloseUI()
    {
        _optionWindow.SetActive(false);
        base.CloseUI();
    }

    public void SetOptionWindow()
    {
        _mainVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.MainVolume;
        _BGMVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.BGMVolume;
        _SFXVolumeBar.GetComponent<Scrollbar>().value = _optionSetting.SFXVolume;

        _fullScreenToggle.GetComponent<Toggle>().isOn = _optionSetting.isFullScreen;
        int curIndex = 0;
        resolutions = new();
        for (int i = 0; i < Screen.resolutions.Length; i++) 
        {
            if (Screen.resolutions[i].width % 10 != 0 || Screen.resolutions[i].height % 10 != 0) continue;

            resolutions.Add(new Vector2Int(Screen.resolutions[i].width, Screen.resolutions[i].height));
            if (Screen.resolutions[i].width == _optionSetting.resolution.x &&
                Screen.resolutions[i].height == _optionSetting.resolution.y) 
            {
                curIndex = i;
            }
        }
        resolutions = resolutions.Distinct().ToList();
        List<string> strings = new();
        foreach (var r in resolutions) strings.Add($"{r.x}x{r.y}");

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

    public void ChangeVolumeOption()
    {
        _optionSetting.MainVolume = _mainVolumeBar.GetComponent<Scrollbar>().value;
        _optionSetting.BGMVolume = _BGMVolumeBar.GetComponent<Scrollbar>().value;
        _optionSetting.SFXVolume = _SFXVolumeBar.GetComponent<Scrollbar>().value;

        SoundManager.instance.SetMainVolume(_optionSetting.MainVolume);
        SoundManager.instance.SetBGMVolume(_optionSetting.BGMVolume);
        SoundManager.instance.SetSFXVolume(_optionSetting.SFXVolume);
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

        UIManager.instance.scriptLanguage = _optionSetting.lauguage;
    }
    public void ResetOption()
    {
        _optionSetting = _initialOptionSetting;
        SetOptionWindow();
        _mainVolumeBar.GetComponent<Scrollbar>().value = _initialOptionSetting.MainVolume;
        _BGMVolumeBar.GetComponent<Scrollbar>().value = _initialOptionSetting.BGMVolume;
        _SFXVolumeBar.GetComponent<Scrollbar>().value = _initialOptionSetting.SFXVolume;

        List<string> laugauages = new() { "Korean", "English" };
        _lauguageDropDown.GetComponent<TMP_Dropdown>().ClearOptions();
        _lauguageDropDown.GetComponent<TMP_Dropdown>().AddOptions(laugauages);
        _lauguageDropDown.GetComponent<TMP_Dropdown>().value = (int)_initialOptionSetting.lauguage - 1;
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
