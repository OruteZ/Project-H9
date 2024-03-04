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
    [SerializeField] private GameObject _baseActionBundle;
    [SerializeField] private GameObject _skillActionBundle;
    [SerializeField] private GameObject _buttonTooltip;

    private const int SKILL_BUTTON_INDEX = 3;

    private GameObject _displayedActionBundle = null;
    private GameObject _activeActionBundle = null;
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
        _baseActionBundle.SetActive(false);
        _skillActionBundle.SetActive(false);
        _buttonTooltip.SetActive(false);
    }
    private void Awake()
    {
        UIManager.instance.onTurnChanged.AddListener(() => SetActionBundle(_displayedActionBundle, false));
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
        _baseActionBundle.GetComponent<RectTransform>().position = screenPos;
        _skillActionBundle.GetComponent<RectTransform>().position = screenPos;

        bool isActive = true;
        if ((Input.GetMouseButtonDown(0) && !IsMouseOverActionUI()) || Input.GetKeyDown(_openKey)) 
        {
            if (Input.GetMouseButtonDown(0) && !IsMouseClickedPlayer())
            {
                isActive = false;
            }
            if (Input.GetKeyDown(_openKey) && _baseActionBundle.activeSelf)
            {
                isActive = false;
            }
            bool isActiveSelectedAction = (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive());
            if (isActiveSelectedAction) return;
            SetActionBundle(_baseActionBundle, isActive);
        }


        if (_activeActionBundle is not null)
        {
            for (int i = 0; i < _shortCutKey.Length; i++)
            {
                if (Input.GetKeyDown(_shortCutKey[i]) && i < _activeActionBundle.transform.childCount)
                {
                    _activeActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().OnClickCombatSelectButton();
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
        CombatActionType[] baseCombatActionType = { CombatActionType.Move, CombatActionType.Attack, CombatActionType.Reload };
        for (int i = 0; i < baseCombatActionType.Length; i++)
        {
            _baseActions.Add(baseCombatActionType[i], ba[i]);
            _baseActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().SetcombatActionButton(baseCombatActionType[i], i, ba[i]);
        }
        _idleAction = ba[3];

        //skill action
        _skillActions.Clear();
        for (int i = 0; i < _skillActionBundle.transform.childCount; i++)
        {
            if (i < actions.Count)
            {
                _skillActions.Add(actions[i]);
                _skillActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().SetcombatActionButton(CombatActionType.PlayerSkill, i, actions[i]);
                _skillActionBundle.transform.GetChild(i).gameObject.SetActive(true);
            }
            else 
            {
                _skillActionBundle.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetActionBundle(GameObject bundle, bool isDisplayed)
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        if (FieldSystem.turnSystem.turnOwner is not Player) return;
        _buttonTooltip.GetComponent<CombatActionButtonTooltip>().CloseUI();
        if (bundle is null) return;
        UpdateButtonSeletable();

        _activeActionBundle = bundle;
        if (isDisplayed)
        {
            _displayedActionBundle = bundle;
            FieldSystem.unitSystem.GetPlayer().SelectAction(_idleAction);
            _selectedActionType = CombatActionType.Null;
        }
        else
        {
            _displayedActionBundle = null;
            //_buttonTooltip.GetComponent<CombatActionButtonTooltip>().CloseUI();
        }
        _baseActionBundle.SetActive(false);
        _skillActionBundle.SetActive(false);
        _backgroundImage.SetActive(isDisplayed);
        if (_displayedActionBundle is not null) _displayedActionBundle.SetActive(isDisplayed);
    }
    public void SelectAction(CombatActionType actionType, int btnIdx)
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
                        SetActionBundle(_displayedActionBundle, false);
                    }
                    else
                    {
                        if (_selectedActionType != actionType)
                        {
                            _selectedActionType = CombatActionType.Null;
                            SelectAction(actionType, btnIdx);
                            return;
                        }
                        SetActionBundle(_activeActionBundle, true);
                    }
                    break;
                }
            case CombatActionType.Skills:
                {
                    SetActionBundle(_skillActionBundle, true);
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
                        SetActionBundle(_displayedActionBundle, false);
                    }
                    else
                    {
                        if (_selectedActionType != actionType)
                        {
                            _selectedActionType = CombatActionType.Null;
                            SelectAction(actionType, btnIdx);
                            return;
                        }
                        SetActionBundle(_activeActionBundle, true);
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

    public bool IsThereSeletableButton()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return true;
        if (player.GetSelectedAction() is not null && player.GetSelectedAction() != _idleAction) return true;

        UpdateButtonSeletable();
        for (int i = 0; i < _baseActionBundle.transform.childCount; i++) 
        {
            GameObject btn = _baseActionBundle.transform.GetChild(i).gameObject;
            if (btn.GetComponent<CombatActionButtonElement>().IsInteractable())
            {
                return true;
            }
        }
        return false;
    }
    private void UpdateButtonSeletable()
    {
        //bool isThereSeletableSkill = false;
        //for (int i = 0; i < _skillActionBundle.transform.childCount; i++)
        //{
        //    GameObject btn = _skillActionBundle.transform.GetChild(i).gameObject;
        //    if (!btn.activeSelf) break;
        //    btn.GetComponent<CombatActionButtonElement>().SetInteractable();
        //    if (btn.GetComponent<CombatActionButtonElement>().IsInteractable()) 
        //    {
        //        isThereSeletableSkill = true;
        //    }
        //}
        bool isSkillExist = _skillActionBundle.transform.GetChild(0).gameObject.activeSelf;

        _baseActionBundle.transform.GetChild(0).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(1).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(2).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(3).GetComponent<CombatActionButtonElement>().SetInteractable(isSkillExist);
        _baseActionBundle.transform.GetChild(4).GetComponent<CombatActionButtonElement>().SetInteractable(false);
        _baseActionBundle.transform.GetChild(5).GetComponent<CombatActionButtonElement>().SetInteractable(false);
    }

    public void ShowRequiredCost(IUnitAction action)
    {
        UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage = action.GetCost();
        UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedMagUsage = action.GetAmmoCost();
        UIManager.instance.onPlayerStatChanged.Invoke();
    }
    public void ClearRequiredCost()
    {
        UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage = 0;
        UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedMagUsage = 0;
        UIManager.instance.onPlayerStatChanged.Invoke();
    }
}
