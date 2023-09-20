using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// �ൿâ UI�� ǥ�� �� �۵��� ���õ� ����� ������ Ŭ����
/// ������: �׼� ��ư UI ��ü�� �ϳ��� Ŭ������ ����ȭ�ؼ� �����ص� �� ��? ������ ���� ������ ����
/// </summary>
public class CombatActionUI : UISystem
{
    [SerializeField] private GameObject _combatActionWindow;
    [SerializeField] private GameObject _actionTooltipWindow;

    private Player _player;
    private GameState _gameState;   //����ȭ ���������� ����. �ذ�Ǹ� �����ص� �� ��?

    private List<GameObject> _actionButtons;
    private GameObject _idleButton;
    private IUnitAction _selectedAction;

    private const int COMBAT_ACTION_TOOLTIP_Y_POSITION_CORRECTION = 150;

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
    public new void Awake()
    {
        base.Awake();

        _gameState = GameState.World;   

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
        _idleButton = _combatActionWindow.transform.GetChild(2).gameObject;

        _idleButton.SetActive(false);
        _actionTooltipWindow.GetComponent<CombatActionTooltip>().CloseUI();

    }

    private void Update()
    {
        for (int i = 0; i <= _actionButtons.Count; i++)
        {
            if (Input.GetKeyDown(_shortCutKey[i]))
            {
                _player = FieldSystem.unitSystem.GetPlayer();
                if (_actionButtons[i].GetComponent<ActionSelectButtonElement>()._action == null)
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
    /// �ൿâ UI�� �׼ǹ�ư���� �����մϴ�.
    /// UI�� ������ ��, �׼��� ���۵� ��, �׼��� ���� ��, �÷��̾ �׼��� ������ �� ����˴ϴ�.
    /// </summary>
    public void SetActionButtons()
    {
        if (_gameState != GameState.Combat) return;
        _player = FieldSystem.unitSystem.GetPlayer();
        if (_player == null) 
        {
            //Debug.Log("�÷��̾ ã�� ���� �ൿâ�� �������� ���߽��ϴ�.");
            return;
        }
        IUnitAction[] playerActions = _player.GetUnitActionArray();

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
                _idleButton.SetActive(false);
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
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            bool isInitButton = (SortedActions.Count > i);
            if (!isInitButton)
            {
                _actionButtons[i].GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
                continue;
            }
            bool isSelectedSomeAction = (selectedActionType != ActionType.Idle);
            if (isSelectedSomeAction)
            {
                bool isActiveAction = _selectedAction.IsActive();
                bool isActiveActionButton = (_actionButtons[i].GetComponent<ActionSelectButtonElement>()._action.GetActionType() == selectedActionType);
                if (!isActiveAction && isActiveActionButton)
                {
                    _idleButton.SetActive(true);
                    _idleButton.transform.position = _actionButtons[i].transform.position;
                    _actionButtons[i].GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
                }
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
            Debug.Log("��ų ���� ���� �ʿ�");
        }

        return actions;
    }

    /// <summary>
    /// �׼� ����â UI�� ���ϴ�.
    /// �׼� ��ư ���� ���콺�� �����ϸ� �� �׼� ��ư ���� �׼ǿ� ���� ������ ����ݴϴ�.
    /// </summary>
    /// <param name="button"></param>
    public void ShowActionUITooltip(GameObject button)
    {
        Vector3 pos = button.transform.position;
        pos.y += COMBAT_ACTION_TOOLTIP_Y_POSITION_CORRECTION;
        _actionTooltipWindow.transform.position = pos;

        IUnitAction action = button.GetComponent<ActionSelectButtonElement>()._action;
        if (action == null) return;

        _actionTooltipWindow.GetComponent<CombatActionTooltip>().SetCombatActionTooltip(action, _selectedAction, pos);
    }
    /// <summary>
    /// �׼� ����â UI�� �ݽ��ϴ�.
    /// �׼� ��ư ������ ���콺 ������ �����ϰ� �ٸ� ���� ����Ű�� ����â�� �ݽ��ϴ�.
    /// </summary>
    public void HideActionUITooltip()
    {
        _actionTooltipWindow.GetComponent<CombatActionTooltip>().CloseUI();
    }

    public bool IsThereSeletableButton() 
    {
        foreach (GameObject btn in _actionButtons) 
        {
            if (btn.GetComponent<ActionSelectButtonElement>()._action == null) continue;
            if (_idleButton.activeSelf) return true;
            if (btn.GetComponent<ActionSelectButtonElement>().IsInteractable()) 
            {
                return true;
            }
        }
        return false;
    }
}