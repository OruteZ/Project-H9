using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// ������ UI ��ü�� �����ϴ� Ŭ����
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

    [Header("Canvases")]
    [SerializeField] private Canvas _worldCanvas;
    [SerializeField] private Canvas _combatCanvas;
    [SerializeField] private Canvas _characterCanvas;
    [SerializeField] private Canvas _skillCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private Canvas _logCanvas;

    //[HideInInspector]
    public bool isMouseOverUI;
    //[HideInInspector]
    public int previousLayer = 1;
    public int currentLayer = 0;

    public GameState UIState { get; private set; }

    public GameObject loading; //test

    [HideInInspector] public UnityEvent<GameState> onTSceneChanged;
    [HideInInspector] public UnityEvent onSceneChanged;
    [HideInInspector] public UnityEvent onTurnChanged;
    [HideInInspector] public UnityEvent onCombatStarted; // UnityEvent<string/int> onSceneChanged �� ���� ��ġ�°� ����
    [HideInInspector] public UnityEvent<Unit> onTurnStarted;
    [HideInInspector] public UnityEvent onPlayerStatChanged;
    [HideInInspector] public UnityEvent<int> onGetExp;
    [HideInInspector] public UnityEvent<int> onLevelUp;
    [HideInInspector] public UnityEvent onPlayerSkillChangd;
    [HideInInspector] public UnityEvent onActionChanged;
    [HideInInspector] public UnityEvent<Unit, int> onTakeDamaged;
    [HideInInspector] public UnityEvent<Unit> onReloaded;
    [HideInInspector] public UnityEvent<Unit> onTryAttack;
    [HideInInspector] public UnityEvent<Unit> onNonHited;
    [HideInInspector] public UnityEvent onInventoryChanged;
    [HideInInspector] public UnityEvent onWeaponChanged;
    [HideInInspector] public UnityEvent<Unit, BaseAction> onStartAction;

    protected override void Awake()
    {
        _worldCanvas.enabled = true;
        _combatCanvas.enabled = false;
        _characterCanvas.enabled = false;
        _skillCanvas.enabled = false;
        _pauseMenuCanvas.enabled = false;
        _logCanvas.enabled = true;

        gameSystemUI = _worldCanvas.GetComponent<GameSystemUI>();
        combatUI = _combatCanvas.GetComponent<CombatWindowUI>();
        characterUI = _characterCanvas.GetComponent<CharacterUI>();
        skillUI = _skillCanvas.GetComponent<SkillUI>();
        pauseMenuUI = _pauseMenuCanvas.GetComponent<PauseMenuUI>();
        debugUI = _debugCanvas.GetComponent<DebugUI>();

        SetCanvasState(_characterCanvas, characterUI, false);
        SetCanvasState(_skillCanvas, skillUI, false);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);

        UIState = GameState.World;
        if (!GameManager.instance.CompareState(UIState)) 
        {
            ChangeScene(GameState.Combat);
        }

        if (loading) loading.SetActive(true);
        base.Awake();
    }
    void Update()
    {
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0))
        {
            currentLayer = GetPointerOverUILayer();
        }
        else
        {
            currentLayer = previousLayer;
        }
        if (Input.GetKeyDown(HotKey.cancelKey))
        {
            Debug.Log("1");
            currentLayer = 1;
        }
        SetUILayer();
    }
    public void SetUILayer()
    {
        if (previousLayer > currentLayer)
        {
            //Debug.Log(previousLayer+" -> "+currentLayer);
            if (currentLayer <= 1)
            {
                SetCharacterCanvasState(false);
                SetSkillCanvasState(false);
                SetPauseMenuCanvasState(false);
                combatUI.ClosePopupWindow();
            }
            else if (currentLayer == 2)
            {
                characterUI.ClosePopupWindow();
                skillUI.ClosePopupWindow();
            }
        }
        previousLayer = currentLayer;
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
    }
    public void SetSkillCanvasState(bool isOn)
    {
        SetCanvasState(_skillCanvas, skillUI, isOn);
    }
    public void SetPauseMenuCanvasState(bool isOn)
    {
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, isOn);
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

    /// <summary>
    /// ���� ��ȯ�Ͽ� UI ���¸� �����մϴ�.
    /// GameManager���� �� ��ȯ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="gameState"> ��ȯ�� ���� �����Ǵ� gameState </param>
    public void ChangeScene(GameState gameState)
    {
        UIState = gameState;
        //if (prevSceneName == currentSceneName) return;
        //Debug.Log("Current State is " + gameState);
        switch (gameState)
        {
            case GameState.World:
                {
                    ChangeUIToWorldScene();
                    break;
                }
            case GameState.Combat:
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

    public void SetUILayerToNormal() 
    {
        currentLayer = 0;
        SetUILayer();
    }

    public float GetCanvasScale() 
    {
        return _worldCanvas.scaleFactor;
    }
}