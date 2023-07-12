using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Generic.Singleton<UIManager>
{
    [HideInInspector]
    public CurrentStatusUI _currentStatusUI { get; private set; }
    public TimingUI _timingUI { get; private set; }
    public QuestUI _questUI { get; private set; }
    public CharacterUI _characterUI { get; private set; }
    public SkillUI _skillUI { get; private set; }
    public PauseMenuUI _pauseMenuUI { get; private set; }

    [Header("Canvases")]
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;
    
    //[HideInInspector]
    public bool isMouseOverUI;
    public int previousLayer = 1;

    void Awake()
    {
        _currentStatusUI = _worldCanvas.GetComponent<CurrentStatusUI>();
        _timingUI = _worldCanvas.GetComponent<TimingUI>();
        _questUI = _worldCanvas.GetComponent<QuestUI>();
        _characterUI = _characterCanvas.GetComponent<CharacterUI>();
        _skillUI = _skillCanvas.GetComponent<SkillUI>();
        _pauseMenuUI = _pauseMenuCanvas.GetComponent<PauseMenuUI>();

        OnOffCanvas(_characterCanvas, _characterUI, false);
        OnOffCanvas(_skillCanvas, _skillUI, false);
        OnOffCanvas(_pauseMenuCanvas, _pauseMenuUI, false);
    }
    void Update()
    {
        isMouseOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0))
        {
            int currentLayer = GetPointerOverUILayer();
            if (previousLayer > currentLayer)
            {
                if (currentLayer <= 1)
                {
                    OnOffCharacterCanvas(false);
                    OnOffSkillCanvas(false);
                    OnOffPauseMenuCanvas(false);
                }
                else if (currentLayer == 2) 
                {
                    _characterUI.ClosePopupWindow();
                    _skillUI.ClosePopupWindow();
                }
            }

            previousLayer = currentLayer;
        }
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

    private int GetPointerOverUILayer() 
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results) 
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI")) 
            {
                return 0;
            }
            else if (result.gameObject.layer == LayerMask.NameToLayer("UI1"))
            {
                return 1;
            }
            else if (result.gameObject.layer == LayerMask.NameToLayer("UI2"))
            {
                return 2;
            }
            else if (result.gameObject.layer == LayerMask.NameToLayer("UI3"))
            {
                return 3;
            }
        }

        return -1;
    }
}
