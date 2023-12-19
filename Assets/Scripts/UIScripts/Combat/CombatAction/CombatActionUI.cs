using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 행동창 UI를 표시 및 작동과 관련된 기능을 구현한 클래스
/// 개선점: 액션 버튼 UI 자체를 하나의 클래스로 조직화해서 관리해도 될 듯? 지금은 조금 번잡한 느낌
/// </summary>
public class CombatActionUI : UISystem
{
    [SerializeField] private GameObject _combatActionWindow;
    [SerializeField] private GameObject _actionTooltipWindow;
    [SerializeField] private GameObject _idleButton;

    private Player _player;
    private GameState _gameState;   //동기화 오류때문에 만듬. 해결되면 삭제해도 될 듯?

    private List<GameObject> _actionButtons;
    private IUnitAction _selectedAction;
    public GameObject selectedButton { get; private set; }

    private KeyCode[] _shortCutKey = 
    {   
            KeyCode.Alpha1, 
            KeyCode.Alpha2, 
            KeyCode.Alpha3, 
            KeyCode.Alpha4, 
            KeyCode.Alpha5, 
            KeyCode.Alpha6, 
            KeyCode.Alpha7, 
            KeyCode.Alpha8, 
            KeyCode.Alpha9, 
            KeyCode.Alpha0,
            KeyCode.Minus,
            KeyCode.Plus
    };

    // Start is called before the first frame update
    private void Awake()
    {
        SetGameState();
        UIManager.instance.onSceneChanged.AddListener(SetGameState);

        //Find Action Buttons & Put in to '_actionButtons'
        _actionButtons = new List<GameObject>();

        Transform baseActionButtons = _combatActionWindow.transform.GetChild(0);
        for (int i = 0; i < baseActionButtons.transform.childCount; i++)
        {
            _actionButtons.Add(baseActionButtons.GetChild(i).gameObject);
        }

        Transform skillActionButtons = _combatActionWindow.transform.GetChild(1);
        for (int i = 0; i < skillActionButtons.transform.childCount; i++)
        {
            _actionButtons.Add(skillActionButtons.GetChild(i).gameObject);
        }

        _idleButton.SetActive(false);
        _actionTooltipWindow.GetComponent<CombatActionTooltip>().CloseUI();

        UIManager.instance.onActionChanged.AddListener(SetActionButtons);
    }

    private void SetGameState() 
    {
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            _gameState = GameState.Combat;
        }
        else
        {
            _gameState = GameState.World;
        }
    }

    private void Update()
    {
        if (_gameState != GameState.Combat) return;
        for (int i = 0; i <= _actionButtons.Count; i++)
        {
            if (Input.GetKeyDown(_shortCutKey[i]))
            {
                _player = FieldSystem.unitSystem.GetPlayer();
                if (_actionButtons[i].GetComponent<ActionSelectButtonElement>().displayedAction == null)
                {
                    _idleButton.GetComponent<ActionSelectButtonElement>().OnClickActionSeleteButton();
                }
                else
                {
                    _actionButtons[i].GetComponent<ActionSelectButtonElement>().OnClickActionSeleteButton();
                }
            }
        }
    }
    public override void OpenUI()
    {
        base.OpenUI();
        _gameState = GameState.Combat;
        SetActionButtons();

    }
    public override void CloseUI()
    {
        base.CloseUI();
        _gameState = GameState.World;
    }
    /// <summary>
    /// 행동창 UI의 액션버튼들을 갱신합니다.
    /// UI를 열었을 때, 액션이 시작될 때, 액션이 끝날 때, 플레이어가 액션을 선택할 때 실행됩니다.
    /// </summary>
    public void SetActionButtons()
    {
        if (_gameState != GameState.Combat) return;
        _player = FieldSystem.unitSystem.GetPlayer();
        if (_player == null) 
        {
            Debug.LogError("플레이어를 찾지 못해 행동창을 구성하지 못했습니다.");
            return;
        }
        IUnitAction[] playerActions = _player.GetUnitActionArray();
        if(playerActions == null) 
        {
            Debug.LogError("플레이어의 액션을 찾지 못해 행동창을 구성하지 못했습니다.");
            return;
        }
        
        
        //Init Button UIs
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            _actionButtons[i].SetActive(true);
        }

        //Find Idle Action
        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].GetActionType() is ActionType.Idle)
            {
                _idleButton.GetComponent<ActionSelectButtonElement>().SetActionSelectButton(playerActions[i], _player);
                _idleButton.SetActive(false); //@@ 
                break;
            }
        }

        //Sort Actions
        List<IUnitAction> SortedActions = SortActions(playerActions);

        //Set Button Info
        for (int i = 0; i < SortedActions.Count; i++)
        {
            _actionButtons[i].GetComponent<ActionSelectButtonElement>().SetActionSelectButton(SortedActions[i], _player);
        }

        //Find Selected Action
        ActionType selectedActionType = ActionType.Idle;
        for (int i = 0; i < SortedActions.Count; i++)
        {
            if (SortedActions[i].GetActionType() == _player.GetSelectedAction().GetActionType())
            {
                _selectedAction = SortedActions[i];
                selectedActionType = SortedActions[i].GetActionType();
                break;
            }
        }

        //Set Button On/Off Status & Set Active Action Button(On Idle Button, Off Seleced Action Button)
        bool isSelectedSomeAction = (selectedActionType != ActionType.Idle);
        if (isSelectedSomeAction)
        {
            _idleButton.SetActive(true);
            _idleButton.transform.position = selectedButton.transform.position;
            selectedButton.GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
        }
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            bool isInitButton = (SortedActions.Count > i);
            if (!isInitButton)
            {
                _actionButtons[i].GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
                continue;
            }
            if(_actionButtons[i] != selectedButton)
            {

            }
        }

    }
    private List<IUnitAction> SortActions(IUnitAction[] playerActions)
    {
        List<IUnitAction> actions = new List<IUnitAction>();
        List<IUnitAction> baseActions = new List<IUnitAction>();
        List<IUnitAction> skillActions = new List<IUnitAction>();
        ActionType[] baseActionType = { ActionType.Move, ActionType.Attack, ActionType.Reload };
        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].GetActionType() is ActionType.Idle)
            {
                continue;
            }
            bool isBaseAction = false;
            for (int j = 0; j < baseActionType.Length; j++) 
            {
                if (playerActions[i].GetActionType() == baseActionType[j])
                {
                    baseActions.Add(playerActions[i]);
                    isBaseAction = true;
                    break;
                }
            }

            if (!isBaseAction)
            {
                skillActions.Add(playerActions[i]);
            }
        }

        int baseActionFindCount = 0;
        while (baseActionFindCount < baseActionType.Length)
        {
            foreach (IUnitAction action in baseActions)
            {
                if (action.GetActionType() == baseActionType[baseActionFindCount])
                {
                    actions.Add(action);
                    baseActionFindCount++;
                    break;
                }
            }
        }
        foreach (IUnitAction action in skillActions)
        {
            actions.Add(action);
        }
        if (actions.Count > _actionButtons.Count)
        {
            Debug.Log("스킬 갯수 조정 필요");
        }

        return actions;
    }

    public void SetSelectedActionButton(GameObject selectedBtn)
    {
        selectedButton = selectedBtn;
    }

    /// <summary>
    /// 액션 설명창 UI를 엽니다.
    /// 액션 버튼 위에 마우스를 오버하면 그 액션 버튼 위에 액션에 대한 설명을 띄워줍니다.
    /// </summary>
    /// <param name="button"></param>
    public void ShowActionUITooltip(GameObject button)
    {
        Vector3 pos = button.GetComponent<RectTransform>().position;
        IUnitAction action = button.GetComponent<ActionSelectButtonElement>().displayedAction;
        if (action == null) return;

        _actionTooltipWindow.GetComponent<CombatActionTooltip>().SetCombatActionTooltip(action, _selectedAction, pos);
    }
    /// <summary>
    /// 액션 설명창 UI를 닫습니다.
    /// 액션 버튼 위에서 마우스 오버를 해제하고 다른 곳을 가리키면 설명창을 닫습니다.
    /// </summary>
    public void HideActionUITooltip()
    {
        _actionTooltipWindow.GetComponent<CombatActionTooltip>().CloseUI();
    }

    public bool IsThereSeletableButton() 
    {
        foreach (GameObject btn in _actionButtons) 
        {
            if (btn.GetComponent<ActionSelectButtonElement>().displayedAction == null) continue;
            if (_idleButton.activeSelf) return true;
            if (btn.GetComponent<ActionSelectButtonElement>().IsInteractable()) 
            {
                return true;
            }
        }
        return false;
    }
}
