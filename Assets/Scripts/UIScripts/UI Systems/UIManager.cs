using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 게임의 UI 전체를 관리하는 클래스
/// </summary>
public class UIManager : Generic.Singleton<UIManager>
{
    [HideInInspector]
    public GameSystemUI gameSystemUI { get; private set; }
    public CombatWindowUI combatUI { get; private set; }
    public CharacterUI characterUI { get; private set; }
    public SkillUI skillUI { get; private set; }
    public PauseMenuUI pauseMenuUI { get; private set; }
    public DebugUI debugUI { get; private set; }
    public InfoPopup infoPopup { get; private set; }

    private readonly string LOCALIZATION_PATH = "UILocalizationTable";
    private Dictionary<int, string> _uiLocalization;
    public Dictionary<int, string> UILocalization => _uiLocalization;

    [Header("Canvases")]
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _combatCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private Canvas _logCanvas;
    [SerializeField] private Canvas _infoPopupCanvas;
    [SerializeField] private Canvas _hotCanvas;
    public Canvas HotCanvas => _hotCanvas;

    //[HideInInspector]
    public bool isMouseOverUI;
    public int currentLayer;/* { get; private set; }*/
    [SerializeField] private GameObject clickMask;

    public GameState UIState { get; private set; }

    public GameObject loading; //test

    public ScriptLanguage scriptLanguage;
    public SystemIconDatabase iconDB;
    public StatScript statScript;

    [HideInInspector] public UnityEvent<GameState> onTSceneChanged;
    [HideInInspector] public UnityEvent onSceneChanged;
    [HideInInspector] public UnityEvent<Unit> onStartedCombatTurn;
    [HideInInspector] public UnityEvent onPlayerStatChanged;
    [HideInInspector] public UnityEvent<int> onGetExp;
    [HideInInspector] public UnityEvent<int> onLevelUp;
    [HideInInspector] public UnityEvent onActionChanged;
    [HideInInspector] public UnityEvent<IDamageable, int, Damage.Type> onTakeDamaged;
    [HideInInspector] public UnityEvent<Unit, int, Damage.Type> onHealed;
    [HideInInspector] public UnityEvent<IDamageable> onNonHited;

    protected override void Awake()
    {
        FileRead.ParseLocalization(in LOCALIZATION_PATH, out _uiLocalization);
        base.Awake();
        if(this == null) return;

        _worldCanvas.enabled = true;
        _combatCanvas.enabled = true;
        _characterCanvas.enabled = true;
        _skillCanvas.enabled = true;
        _pauseMenuCanvas.enabled = true;
        _logCanvas.enabled = true;
        _infoPopupCanvas.enabled = true;

        gameSystemUI = _worldCanvas.GetComponent<GameSystemUI>();
        combatUI = _combatCanvas.GetComponent<CombatWindowUI>();
        characterUI = _characterCanvas.GetComponent<CharacterUI>();
        skillUI = _skillCanvas.GetComponent<SkillUI>();
        pauseMenuUI = _pauseMenuCanvas.GetComponent<PauseMenuUI>();
        debugUI = _debugCanvas.GetComponent<DebugUI>();
        infoPopup = _infoPopupCanvas.GetComponent<InfoPopup>();

        UIState = GameState.WORLD;
        if (!GameManager.instance.CompareState(UIState)) 
        {
            ChangeScene(GameState.COMBAT);
        }
        if (loading) loading.SetActive(true);

        StartCoroutine(SetStatScript());

        if (Service.IsMissingRoot())
            Service.CreateRoot();
    }
    private IEnumerator SetStatScript() 
    {
        if (scriptLanguage == ScriptLanguage.NULL) yield return null;
        statScript = new StatScript();
        yield break;
    }

    private void FieldAwake()
    {
        FieldSystem.turnSystem.onTurnChanged.AddListener(() => currentLayer = 1);
    }
    
    private void Start()
    {
        _worldCanvas.enabled = true;
        _combatCanvas.enabled = false;
        _characterCanvas.enabled = false;
        _skillCanvas.enabled = false;
        _pauseMenuCanvas.enabled = false;
        _logCanvas.enabled = true;
        _infoPopupCanvas.enabled = true;

        SetCanvasState(_characterCanvas, characterUI, false);
        SetCanvasState(_skillCanvas, skillUI, false);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);
    }
    void Update()
    {
        //Debug.Log(currentLayer);
        isMouseOverUI = (EventSystem.current.IsPointerOverGameObject()/* || currentLayer > 1*/);
        //if (combatUI.combatActionUI.isCombatUIOpened()) return;
        if (!combatUI.combatActionUI.IsMouseOverActionUI() && Input.GetMouseButtonDown(0))
        {
            int layer = GetPointerOverUILayer();
            if (layer < 2 && !_characterCanvas.enabled && !_skillCanvas.enabled && !_pauseMenuCanvas.enabled)
            {
                gameSystemUI.CloseUI();
            }
            SetUILayer(layer);
        }
        else if (Input.GetKeyDown(HotKey.cancelKey))
        {
            SetUILayer(currentLayer - 1);
        }
    }
    public void SetUILayer(int layerLevel)
    {
        if (currentLayer == layerLevel) return;
        currentLayer = layerLevel;
        //Debug.LogError(layerLevel > 1);
        clickMask.SetActive(layerLevel > 1 && GameManager.instance.CompareState(GameState.WORLD));

        switch (currentLayer) 
        {
            case 0: 
                {
                    SetPauseMenuCanvasState(true);
                    currentLayer = 2;
                    break;
                }
            case 1:
                {
                    SetCanvasState(_characterCanvas, characterUI, false);
                    SetCanvasState(_skillCanvas, skillUI, false);
                    SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);
                    combatUI.CloseUI();
                    break;
                }
            case 2:
                {
                    characterUI.ClosePopupWindow();
                    skillUI.ClosePopupWindow();
                    break;
                }
            case 3:
                {
                    break;
                }
        }
    }

    private void SetCanvasState(Canvas canvas, UISystem uiSys, bool isOn)
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
    public void SetCombatCanvasState(bool isOn)
    {
        SetCanvasState(_combatCanvas, combatUI, isOn);
    }
    public void SetCharacterCanvasState(bool isOn)
    {
        SetCanvasState(_characterCanvas, characterUI, isOn);
        SetCanvasState(_skillCanvas, skillUI, false);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);
        if (!_characterCanvas.enabled) SetUILayer(1);
        SoundManager.instance.PlaySFX("UI_WindowOpen");
    }
    public void SetSkillCanvasState(bool isOn)
    {
        SetCanvasState(_characterCanvas, characterUI, false);
        SetCanvasState(_skillCanvas, skillUI, isOn);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);
        if (!_skillCanvas.enabled) SetUILayer(1);
        SoundManager.instance.PlaySFX("UI_WindowOpen");
    }
    public void SetPauseMenuCanvasState(bool isOn)
    {
        SetCanvasState(_characterCanvas, characterUI, false);
        SetCanvasState(_skillCanvas, skillUI, false);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, isOn);
        if (!_pauseMenuCanvas.enabled) currentLayer = 1;
        SoundManager.instance.PlaySFX("UI_WindowOpen");
    }
    public bool GetCharacterCanvasState()
    {
        return _characterCanvas.enabled;
    }
    public bool GetSkillCanvasState()
    {
        return _skillCanvas.enabled;
    }

    public void OnExitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
                return 1;
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

        return 1;
    }

    /// <summary>
    /// 씬을 전환하여 UI 상태를 변경합니다.
    /// GameManager에서 씬 전환 시 호출됩니다.
    /// </summary>
    /// <param name="gameState"> 전환할 씬에 대응되는 gameState </param>
    public void ChangeScene(GameState gameState)
    {
        UIState = gameState;
        //if (prevSceneName == currentSceneName) return;
        //Debug.Log("Current State is " + gameState);
        switch (gameState)
        {
            case GameState.WORLD:
                {
                    ChangeUIToWorldScene();
                    break;
                }
            case GameState.COMBAT:
                {
                    ChangeUIToCombatScene();
                    break;
                }
        }
        onTSceneChanged?.Invoke(gameState);
        onSceneChanged?.Invoke();
    }
    private void ChangeUIToWorldScene()
    {
        SetCombatCanvasState(false);
    }
    private void ChangeUIToCombatScene()
    {
        SetCombatCanvasState(true);
    }

    public float GetCanvasScale() 
    {
        return _worldCanvas.scaleFactor;
    }

    public void SetLogCanvasState(bool isOpen) 
    {
        _logCanvas.enabled = isOpen;
    }

    public void SetUILanguage(ScriptLanguage language) 
    {
        scriptLanguage = language;
        UserAccount.Language = language;
        FileRead.ParseLocalization(in LOCALIZATION_PATH, out _uiLocalization);
    }
}

public enum ScriptLanguage
{
    NULL,
    Korean,
    English
}