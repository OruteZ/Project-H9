using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatActionUI : UISystem
{
    [SerializeField] private GameObject _combatActionWindow;
    [SerializeField] private GameObject _actionTooltipWindow;

    private Player _player;
    private GameState _gameState;

    private readonly List<GameObject> _actionButtons = new List<GameObject>();
    private GameObject _idleButton;
    private IUnitAction _activeAction;
    // Start is called before the first frame update
    public new void Awake()
    {
        base.Awake();

        _gameState = GameState.World;

        //Find Action Buttons & Put in to '_actionButtons'
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
        _actionTooltipWindow.SetActive(false);

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
    public void SetActionButtons()
    {
        if (_gameState != GameState.Combat) return;
        _player = FieldSystem.unitSystem.GetPlayer();
        IUnitAction[] playerActions = _player.GetUnitActionArray();

        //Init Button UIs
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            _actionButtons[i].SetActive(true);
            _actionButtons[i].GetComponent<Button>().interactable = true;
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
        List<IUnitAction> actions = SortActions(playerActions);

        //Set Button Info
        for (int i = 0; i < actions.Count; i++)
        {
            _actionButtons[i].GetComponent<ActionSelectButtonElement>().SetActionSelectButton(actions[i], _player);
        }

        //Find Active Action
        ActionType activeActionType = ActionType.Idle;
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].GetActionType() == _player.GetSelectedAction().GetActionType())
            {
                _activeAction = actions[i];
                activeActionType = actions[i].GetActionType();
                break;
            }
        }

        //Set Button Status & Set Active Action Button
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            bool isInitButton = (actions.Count > i);
            bool isActiveSomeAction = (activeActionType != ActionType.Idle);
            if (!isInitButton)
            {
                _actionButtons[i].GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
                continue;
            }
            if (isActiveSomeAction)
            {
                if (_actionButtons[i].GetComponent<ActionSelectButtonElement>()._action.GetActionType() == activeActionType)
                {
                    _idleButton.SetActive(true);
                    _idleButton.transform.position = _actionButtons[i].transform.position;
                    if (_activeAction.IsActive()) 
                    {
                        _idleButton.GetComponent<ActionSelectButtonElement>().OffActionSelectButton();
                    }
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
        int baseActionFindCount = 0;
        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].GetActionType() is ActionType.Idle)
            {
                continue;
            }
            else if (baseActionFindCount < baseActionType.Length && playerActions[i].GetActionType() == baseActionType[baseActionFindCount])
            {
                baseActions.Add(playerActions[i]);
                baseActionFindCount++;
            }
            else
            {
                skillActions.Add(playerActions[i]);
            }
        }
        baseActionFindCount = 0;
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

    public void ShowActionUITooltip(GameObject button) 
    {
        _actionTooltipWindow.SetActive(true);

        Vector3 pos = button.transform.position;
        pos.y += 200;
        _actionTooltipWindow.transform.position = pos;

        IUnitAction action = button.GetComponent<ActionSelectButtonElement>()._action;
        if (action == null) return;

        string actionName = action.GetActionType().ToString();
        //string actionDescription = action.GetActionDescription().ToString();
        string actionDescription = "Description";
        if (actionName == "Idle") 
        {
            actionName = "Cancel " + _activeAction.GetActionType().ToString();
            //actionDescription = _activeAction.GetActionDescription().ToString();
        }
        _actionTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionName;
        _actionTooltipWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = actionDescription;
    }
    public void HideActionUITooltip()
    {
        _actionTooltipWindow.SetActive(false);
        _actionTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
}
