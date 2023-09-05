using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 UI 전체를 관리하는 클래스
/// </summary>
public class UIManager : Generic.Singleton<UIManager>
{
    [HideInInspector]
    public CurrentStatusUI currentStatusUI { get; private set; }
    public TimingUI timingUI { get; private set; }
    public QuestUI questUI { get; private set; }
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

    //[HideInInspector] //테스트용으로 켜둠.
    public bool isMouseOverUI;
    public int previousLayer = 1;

    private GameState _UIState;

    public GameObject loading; //test

    private new void Awake()
    {
        if(loading is not null) loading.SetActive(true);

        base.Awake();
        _combatCanvas.enabled = false;
        _characterCanvas.enabled = false;
        _skillCanvas.enabled = false;
        _pauseMenuCanvas.enabled = false;

        currentStatusUI = _worldCanvas.GetComponent<CurrentStatusUI>();
        timingUI = _worldCanvas.GetComponent<TimingUI>();
        questUI = _worldCanvas.GetComponent<QuestUI>();

        combatUI = _combatCanvas.GetComponent<CombatWindowUI>();

        characterUI = _characterCanvas.GetComponent<CharacterUI>();
        skillUI = _skillCanvas.GetComponent<SkillUI>();
        pauseMenuUI = _pauseMenuCanvas.GetComponent<PauseMenuUI>();
        debugUI = _debugCanvas.GetComponent<DebugUI>();

        SetCanvasState(_characterCanvas, characterUI, false);
        SetCanvasState(_skillCanvas, skillUI, false);
        SetCanvasState(_pauseMenuCanvas, pauseMenuUI, false);

        _UIState = GameState.World;
        if (!GameManager.instance.CompareState(_UIState)) 
        {
            ChangeScene(GameState.Combat);
        }
    }
    void Update()
    {
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0))
        {
            int currentLayer = GetPointerOverUILayer();
            if (previousLayer > currentLayer)
            {
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
    }

    //아래 캔버스 켜고끄는 것들 UIInteraction 삭제 후 전부 private로 전환 예정.
    public void SetCanvasState(Canvas canvas, UISystem uiSys, bool isOn)
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
    /// 씬을 전환하여 UI 상태를 변경합니다.
    /// GameManager에서 씬 전환 시 호출됩니다.
    /// </summary>
    /// <param name="gameState"> 전환할 씬에 대응되는 gameState </param>
    public void ChangeScene(GameState gameState)
    {
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

    }
    private void ChangeUIToWorldScene()
    {
        SetCombatCanvasState(false);
        timingUI.SetTurnOrderUIState(false);
    }
    private void ChangeUIToCombatScene()
    {
        SetCombatCanvasState(true);
        timingUI.SetTurnOrderUIState(true);
    }
}
