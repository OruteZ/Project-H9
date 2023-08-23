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

    private Player _player;
    private GameState _gameState;   //동기화 오류때문에 만듬. 해결되면 삭제해도 될 듯?

    private List<GameObject> _actionButtons;
    private GameObject _idleButton;
    private IUnitAction _activeAction;
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
            //Debug.Log("플레이어를 찾지 못해 행동창을 구성하지 못했습니다.");
            return;
        }
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
                bool isActiveAction = _activeAction.IsActive();
                bool isActiveActionButton = (_actionButtons[i].GetComponent<ActionSelectButtonElement>()._action.GetActionType() == activeActionType);
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
            Debug.Log("스킬 갯수 조정 필요");
        }

        return actions;
    }

    /// <summary>
    /// 액션 설명창 UI를 엽니다.
    /// 액션 버튼 위에 마우스를 오버하면 그 액션 버튼 위에 액션에 대한 설명을 띄워줍니다.
    /// </summary>
    /// <param name="button"></param>
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
    /// <summary>
    /// 액션 설명창 UI를 닫습니다.
    /// 액션 버튼 위에서 마우스 오버를 해제하고 다른 곳을 가리키면 설명창을 닫습니다.
    /// </summary>
    public void HideActionUITooltip()
    {
        _actionTooltipWindow.SetActive(false);
        _actionTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }
}
