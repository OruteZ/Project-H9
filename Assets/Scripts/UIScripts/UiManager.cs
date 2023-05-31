using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiManager : Generic.Singleton<UiManager>
{
    public SkillUI _skillUI { get; private set; }
    public CharacterUI _characterUI { get; private set; }

    //public PauseMenuUI _pauseMenuUI { get; private set; }

    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;

    [SerializeField] private GameObject _backgroundButton;

    public bool isMouseOverUI;

    void Start()
    {
        _skillUI = GetComponent<SkillUI>();
        _characterUI = GetComponent<CharacterUI>();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isMouseOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
    }

    public void OnOffCharacterCanvas(bool isOn)
    {
        if (isOn && _characterCanvas.enabled) isOn = false;
        _characterCanvas.enabled = isOn;

        if (_characterCanvas.enabled)
        {
            _characterUI.SetLearnedSkiilInfoUI();
        }
        
        OnOffBackgroundBtn();
    }
    public void OnOffSkillCanvas(bool isOn)
    {
        if (isOn && _skillCanvas.enabled) isOn = false;
        _skillCanvas.enabled = isOn;

        if (!_skillCanvas.enabled)
        {
            _skillUI.CloseSkillTooltip();
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
    
}
