using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CombatActionType 
{
    Null =      0,
    Move =      1,
    Attack =    2,
    Reload =    3,
    Skills =    4,
    Items =     5,
    Run =       6,
    PlayerSkill = 7
}

public class CombatActionUI : UISystem
{
    [SerializeField] private GameObject _backgroundImage;
    [SerializeField] private GameObject _baseActionSet;
    [SerializeField] private GameObject _skillActionSet;
    [SerializeField] private GameObject _buttonTooltip;

    private GameObject _displayedActionSet = null;
    private GameObject _activeActionSet = null;
    private Dictionary<CombatActionType, IUnitAction> _baseActions = new Dictionary<CombatActionType, IUnitAction>();
    private List<IUnitAction> _skillActions = new List<IUnitAction>();
    private IUnitAction _idleAction;
    private CombatActionType _selectedActionType = CombatActionType.Null;

    private KeyCode _openKey = KeyCode.Tab;
    private KeyCode[] _shortCutKey =
    {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8
    };
    public override void CloseUI()
    {
        base.CloseUI();
        _backgroundImage.SetActive(false);
        _baseActionSet.SetActive(false);
        _skillActionSet.SetActive(false);
        _buttonTooltip.SetActive(false);
    }
    private void Awake()
    {
        UIManager.instance.onTurnChanged.AddListener(() => SetActionSet(_displayedActionSet, false));
        UIManager.instance.onSceneChanged.AddListener(InitActions);
    }
    private void Start()
    {
        CloseUI();
    }

    void Update()
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;

        Vector3 playerChestPosition = player.transform.position;
        if (!player.TryGetComponent(out CapsuleCollider var)) return;
        playerChestPosition.y += player.GetComponent<CapsuleCollider>().center.y;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(playerChestPosition);
        _backgroundImage.GetComponent<RectTransform>().position = screenPos;
        _baseActionSet.GetComponent<RectTransform>().position = screenPos;
        _skillActionSet.GetComponent<RectTransform>().position = screenPos;

        bool isActive = true;
        if ((Input.GetMouseButtonDown(0) && !IsMouseOverActionUI()) || Input.GetKeyDown(_openKey)) 
        {
            if (Input.GetMouseButtonDown(0) && !IsMouseClickedPlayer())
            {
                isActive = false;
            }
            if (Input.GetKeyDown(_openKey) && _baseActionSet.activeSelf)
            {
                isActive = false;
            }
            bool isActiveSelectedAction = (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive());
            if (isActiveSelectedAction) return;
            SetActionSet(_baseActionSet, isActive);
        }


        if (_activeActionSet is not null)
        {
            for (int i = 0; i < _shortCutKey.Length; i++)
            {
                if (Input.GetKeyDown(_shortCutKey[i]) && i < _activeActionSet.transform.childCount)
                {
                    _activeActionSet.transform.GetChild(i).GetComponent<CombatActionButtonElement>().OnClickCombatSelectButton();
                    break;
                }
            }
        }
    }
    private void InitActions()
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;

        List<IUnitAction> actions = new List<IUnitAction>(player.GetUnitActionArray());
        ActionType[] baseActionType = { ActionType.Move, ActionType.Attack, ActionType.Reload, ActionType.Idle };

        List<IUnitAction> ba = new List<IUnitAction>();
        for (int j = 0; j < baseActionType.Length; j++)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].GetActionType() == baseActionType[j])
                {
                    ba.Add(actions[i]);
                    actions.RemoveAt(i);
                    i--;
                }
            }
        }

        //basic action
        if (ba.Count != baseActionType.Length)
        {
            Debug.LogError("Player에 기본 액션이 없습니다." + ba.Count + " != " + baseActionType.Length);
            return;
        }
        _baseActions.Clear();
        _baseActions.Add(CombatActionType.Move, ba[0]);
        _baseActions.Add(CombatActionType.Attack, ba[1]);
        _baseActions.Add(CombatActionType.Reload, ba[2]);
        _idleAction = ba[3];

        //skill action
        _skillActions.Clear();
        for (int i = 0; i < _skillActionSet.transform.childCount; i++)
        {
            if (i < actions.Count)
            {
                _skillActions.Add(actions[i]);
                _skillActionSet.transform.GetChild(i).GetComponent<CombatActionButtonElement>().SetcombatActionButton(CombatActionType.PlayerSkill, i, actions[i].GetActionType());
                _skillActionSet.transform.GetChild(i).gameObject.SetActive(true);
            }
            else 
            {
                _skillActionSet.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetActionSet(GameObject set, bool isDisplayed)
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        if (FieldSystem.turnSystem.turnOwner is not Player) return;
        _buttonTooltip.GetComponent<CombatActionButtonTooltip>().CloseUI();
        if (set is null) 
        {
            return;
            _activeActionSet = null;
            _displayedActionSet = null;
            FieldSystem.unitSystem.GetPlayer().SelectAction(_idleAction);

        }

        _activeActionSet = set;
        if (isDisplayed)
        {
            _displayedActionSet = set;
            FieldSystem.unitSystem.GetPlayer().SelectAction(_idleAction);
            _selectedActionType = CombatActionType.Null;
        }
        else
        {
            _displayedActionSet = null;
            //_buttonTooltip.GetComponent<CombatActionButtonTooltip>().CloseUI();
        }
        _baseActionSet.SetActive(false);
        _skillActionSet.SetActive(false);
        _backgroundImage.SetActive(isDisplayed);
        if (_displayedActionSet is not null) _displayedActionSet.SetActive(isDisplayed);
    }
    public void TakeAction(CombatActionType actionType, int btnIdx)
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (FieldSystem.turnSystem.turnOwner is not Player) return;
        if (player.GetSelectedAction().IsActive()) return;

            switch (actionType)
        {
            case CombatActionType.Move:
            case CombatActionType.Attack:
            case CombatActionType.Reload:
                {
                    if (_selectedActionType == CombatActionType.Null)
                    {
                        FieldSystem.unitSystem.GetPlayer().SelectAction(_baseActions[actionType]);
                        _selectedActionType = actionType;
                        SetActionSet(_displayedActionSet, false);
                    }
                    else
                    {
                        if (_selectedActionType != actionType)
                        {
                            _selectedActionType = CombatActionType.Null;
                            TakeAction(actionType, btnIdx);
                            return;
                        }
                        SetActionSet(_activeActionSet, true);
                    }
                    break;
                }
            case CombatActionType.Skills:
                {
                    SetActionSet(_skillActionSet, true);
                    break;
                }
            case CombatActionType.Items:
                {
                    Debug.Log("Item Button Clicked");
                    break;
                }
            case CombatActionType.Run:
                {
                    Debug.Log("Run Button Clicked");
                    break;
                }
            case CombatActionType.PlayerSkill:
                {
                    if (_selectedActionType == CombatActionType.Null)
                    {
                        FieldSystem.unitSystem.GetPlayer().SelectAction(_skillActions[btnIdx]);
                        _selectedActionType = actionType;
                        SetActionSet(_displayedActionSet, false);
                    }
                    else
                    {
                        if (_selectedActionType != actionType)
                        {
                            _selectedActionType = CombatActionType.Null;
                            TakeAction(actionType, btnIdx);
                            return;
                        }
                        SetActionSet(_activeActionSet, true);
                    }
                    break;
                }
        }
    }

    public void ShowActionUITooltip(GameObject btn)
    {
        _buttonTooltip.GetComponent<CombatActionButtonTooltip>().SetCombatActionTooltip(btn);
    }
    public void HideActionUITooltip() 
    {
        _buttonTooltip.GetComponent<CombatActionButtonTooltip>().CloseUI();
    }
    private static bool IsMouseClickedPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isSuccessRaycast = Physics.Raycast(ray, float.MaxValue, layerMask: LayerMask.GetMask("Player"));
        if (isSuccessRaycast)
        {
            return true;
        }

        return false;
    }
    private bool IsMouseOverActionUI()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        if (results.Count != 0)
        {
            return true;
        }

        return false;
    }
}
