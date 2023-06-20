using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Generic.Singleton<UIManager>
{
    [HideInInspector]
    public CurrentStatusUI _currentStatusUI { get; private set; }
    public TimingUI _timingUI { get; private set; }
    public QuestUI _questUI { get; private set; }
    public CharacterUI _characterUI { get; private set; }
    public SkillUI _skillUI { get; private set; }
    public PauseMenuUI _pauseMenuUI { get; private set; }

    [Header("Managers")]
    public SkillManager _skillManager;
    public ItemManager _itemManager;

    [Header("Canvases")]
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;

    [Header("Others")]
    [SerializeField] private GameObject _backgroundButton;
    
    //[HideInInspector]
    public bool isMouseOverUI;

    void Start()
    {
        _currentStatusUI = _worldCanvas.GetComponent<CurrentStatusUI>();
        _timingUI = GetComponent<TimingUI>();
        _questUI = GetComponent<QuestUI>();
        _characterUI = _characterCanvas.GetComponent<CharacterUI>();
        _skillUI = _skillCanvas.GetComponent<SkillUI>();
        _pauseMenuUI = _pauseMenuCanvas.GetComponent<PauseMenuUI>();
    }
    void Update()
    {
        isMouseOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    public void OnOffCanvas(Canvas canvas, UISystem uiSys, bool isOn)
    {
        if (canvas.enabled && isOn)
        {
            isOn = false;
        }

        if (canvas.enabled != isOn) 
        {
            if (isOn)
            {
                canvas.enabled = isOn;
                uiSys.OpenUI();
            }
            else 
            {
                uiSys.CloseUI();
                canvas.enabled = isOn;
            }
            OnOffBackgroundBtn();
        }
    }
    public void OnOffCharacterCanvas(bool isOn)
    {
        OnOffCanvas(_characterCanvas, _characterUI, isOn);
    }
    public void OnOffSkillCanvas(bool isOn)
    {
        OnOffCanvas(_skillCanvas, _skillUI, isOn);
    }
    public void OnOffPauseMenuCanvas(bool isOn)
    {
        OnOffCanvas(_pauseMenuCanvas, _pauseMenuUI, isOn);
    }
    private void OnOffBackgroundBtn()
    {
        bool isActiveSomeWindow = false;
        if (_characterCanvas.enabled) isActiveSomeWindow = true;
        if (_skillCanvas.enabled) isActiveSomeWindow = true;

        _backgroundButton.SetActive(isActiveSomeWindow);
    }
    
}
