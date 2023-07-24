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

    private List<GameObject> _actionButtons = new List<GameObject>();
    private GameObject _idleButton;
    // Start is called before the first frame update
    void Start()
    {
        _gameState = GameState.World;

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

    // Update is called once per frame
    void Update()
    {

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
        IUnitAction[] actions = _player.GetUnitActionArray();

        for (int i = 0; i < _actionButtons.Count; i++)
        {
            _actionButtons[i].SetActive(true);
            _actionButtons[i].GetComponent<Button>().interactable = true;
        }

        ActionType activeActionType = ActionType.Idle;
        int actionButtonsIndex = 0;
        for (int i = 0; i < actions.Length; i++)
        {
            //idle action exception
            if (actions[i].GetActionType() == ActionType.Idle) 
            {
                _idleButton.GetComponent<ActionSelectButtonElements>().SetActionSelectButton(actions[i], _player);
                _idleButton.SetActive(false);
                continue;
            }
            //find active action
            if (actions[i].GetActionType() == _player.GetSelectedAction().GetActionType()) 
            {
                activeActionType = actions[i].GetActionType();
            }
            //set button info
            _actionButtons[actionButtonsIndex].GetComponent<ActionSelectButtonElements>().SetActionSelectButton(actions[i], _player);
            actionButtonsIndex++;
        }

        //Button Status Init & Active Action Setting
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            bool isInitButton = (actionButtonsIndex > i);
            if (!isInitButton)
            {
                _actionButtons[i].GetComponent<Button>().interactable = false;
            }
            else if (activeActionType != ActionType.Idle)
            {
                _actionButtons[i].GetComponent<Button>().interactable = false;
                if (_actionButtons[i].GetComponent<ActionSelectButtonElements>()._action.GetActionType() == activeActionType)
                {
                    _actionButtons[i].SetActive(false);
                    _idleButton.SetActive(true);
                    _idleButton.transform.position = _actionButtons[i].transform.position;
                }
            }
        }
    }

    public void ShowActionUITooltip(GameObject button) 
    {
        _actionTooltipWindow.SetActive(true);

        Vector3 pos = button.transform.position;
        pos.y += 200;
        _actionTooltipWindow.transform.position = pos;

        IUnitAction action = button.GetComponent<ActionSelectButtonElements>()._action;
        if (action == null) return;
        _actionTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = action.GetActionType().ToString();
    }
    public void HideActionUITooltip()
    {
        _actionTooltipWindow.SetActive(false);
    }
}
