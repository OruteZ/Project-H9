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
    Weapons =   6,
    PlayerSkill = 7
}

public class CombatActionUI : UISystem
{
    [SerializeField] private GameObject _combatHUD;
    [SerializeField] private GameObject _backgroundImage;
    [SerializeField] private GameObject _baseActionBundle;
    [SerializeField] private GameObject _skillActionBundle;
    [SerializeField] private GameObject _buttonNameTooltip;
    [SerializeField] private GameObject _skillTooltip;
    [SerializeField] private GameObject _itemWindow;
    [SerializeField] private GameObject _itemElements;
    [SerializeField] private GameObject _itemTooltip;

    private const int SKILL_BUTTON_INDEX = 3;
    private bool _isThereSeletableSkill = false;

    private GameObject _displayedActionBundle = null;
    private GameObject _activeActionBundle = null;
    private Dictionary<CombatActionType, IUnitAction> _baseActions = new Dictionary<CombatActionType, IUnitAction>();
    private List<IUnitAction> _skillActions = new List<IUnitAction>();
    private IUnitAction _idleAction;
    private IUnitAction _itmeUsingAction;
    private CombatActionType _selectedActionType = CombatActionType.Null;

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
    public bool isCombatUIOpened() 
    {
        return !(_displayedActionBundle == null && _activeActionBundle == null);
    }
    public override void CloseUI()
    {
        SetActionBundle(null, null);
        base.CloseUI();
    }
    private void Awake()
    {
        UIManager.instance.onSceneChanged.AddListener(InitActions);
    }
    private void Start()
    {
        _backgroundImage.SetActive(false);
        _baseActionBundle.SetActive(false);
        _skillActionBundle.SetActive(false);
        _buttonNameTooltip.SetActive(false);
        _skillTooltip.SetActive(false);
        _itemWindow.SetActive(false);
        CloseUI();
    }

    void Update()
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;

        //hud position setting
        Vector3 playerChestPosition = player.transform.position;
        if (!player.TryGetComponent(out CapsuleCollider var)) return;
        playerChestPosition.y += player.GetComponent<CapsuleCollider>().center.y;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(playerChestPosition);
        _combatHUD.GetComponent<RectTransform>().position = screenPos;

        SetCombatActionUIState();

        //shortcut key
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
    private void SetCombatActionUIState()
    {
        bool isActiveSelectedAction = (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive());
        if (!isActiveSelectedAction)
        {
            bool isPlayerClicked = !IsMouseOverActionUI() && (Input.GetMouseButtonDown(0) && IsMouseClickedPlayer());
            bool isOpenKeyClicked = Input.GetKeyDown(HotKey.openActionUIKey);
            bool isCancelKeyClicked = Input.GetKeyDown(HotKey.cancelKey);
            if (isPlayerClicked || isOpenKeyClicked || isCancelKeyClicked)
            {
                Debug.Log(isPlayerClicked + " / " + isOpenKeyClicked + " / " + isCancelKeyClicked);
            }
            //open key
            if (isOpenKeyClicked || isPlayerClicked)
            {
                Debug.Log(_activeActionBundle + " / " + _displayedActionBundle);
                if (_activeActionBundle == null && _displayedActionBundle == null)
                {
                    SetActionBundle(_baseActionBundle, _baseActionBundle);
                    return;
                }
                else if (_activeActionBundle == _displayedActionBundle)
                {
                    SetActionBundle(null, null);
                    return;
                }
            }
            //cancel key
            if (isCancelKeyClicked || isPlayerClicked)
            {
                if (_activeActionBundle != null && _displayedActionBundle == null)
                {
                    SetActionBundle(_activeActionBundle, _activeActionBundle);
                }
                else if (_displayedActionBundle == _skillActionBundle)
                {
                    SetActionBundle(_baseActionBundle, _baseActionBundle);
                }
                else if (_displayedActionBundle == _baseActionBundle)
                {
                    SetActionBundle(null, null);
                }
            }
        }
    }
    private void InitActions()
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;
        UIManager.instance.onStartedCombatTurn.AddListener((u) => { if (u is Player) { SetActionBundle(_baseActionBundle, _baseActionBundle); } else { SetActionBundle(null, null); } });
        FieldSystem.onCombatFinish.AddListener((b) =>SetActionBundle(null, null));
        player.onTurnStart.AddListener((u) => { SetActionBundle(_baseActionBundle, _baseActionBundle); });
        player.onFinishAction.AddListener((a) => { if (a is not IdleAction && IsThereSeletableButton()) { SetActionBundle(_baseActionBundle, _baseActionBundle); } });

        LoadPlayerAction();
    }
    private void LoadPlayerAction()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;
        List<IUnitAction> actions = new List<IUnitAction>(player.GetUnitActionArray());
        ActionType[] baseActionType = { ActionType.Move, ActionType.Attack, ActionType.Reload, ActionType.Idle, ActionType.ItemUsing };

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

        for (int i = 0; i < _baseActionBundle.transform.childCount; i++)
        {
            _baseActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().ClearCombatActionButton();
        }
        for (int i = 0; i < _skillActionBundle.transform.childCount; i++)
        {
            _skillActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().ClearCombatActionButton();
        }

        //basic action
        if (ba.Count != baseActionType.Length)
        {
            Debug.LogError("Player에 기본 액션이 없거나 잘 못 들어가 있습니다." + ba.Count + " != " + baseActionType.Length);
            return;
        }
        _baseActions.Clear();
        CombatActionType[] baseCombatActionType = { CombatActionType.Move, CombatActionType.Attack, CombatActionType.Reload };
        for (int i = 0; i < baseCombatActionType.Length; i++)
        {
            _baseActions.Add(baseCombatActionType[i], ba[i]);
            _baseActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().SetCombatActionButton(baseCombatActionType[i], i, ba[i]);
        }
        _idleAction = ba[3];
        _itmeUsingAction = ba[4];

        //skill action
        _skillActions.Clear();
        for (int i = 0; i < _skillActionBundle.transform.childCount; i++)
        {
            if (i < actions.Count)
            {
                _skillActions.Add(actions[i]);
                _skillActionBundle.transform.GetChild(i).GetComponent<CombatActionButtonElement>().SetCombatActionButton(CombatActionType.PlayerSkill, i, actions[i]);
                _skillActionBundle.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                _skillActionBundle.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetActionBundle(GameObject activeBundle, GameObject displayedBundle)
    {
        if (UIManager.instance.UIState != GameState.Combat) return;
        if (FieldSystem.turnSystem.turnOwner is not Player) return;
        if (!FieldSystem.unitSystem.isEnemyExist()) return;
        _buttonNameTooltip.GetComponent<CombatActionNameTooltip>().CloseUI();
        //Debug.Log(activeBundle + " / " + displayedBundle);
        _activeActionBundle = activeBundle;
        _displayedActionBundle = displayedBundle;

        bool isDisplayed = (_displayedActionBundle != null);
        bool isUIClosed = (_activeActionBundle == null && _displayedActionBundle == null);
        if (isUIClosed)
        {
            UIManager.instance.SetUILayer(1);
        }
        else
        {
            UIManager.instance.SetUILayer(2);
        }
        Player player = FieldSystem.unitSystem.GetPlayer();
        if ((isDisplayed || isUIClosed) && player is not null /*&& player.GetSelectedAction() is not ItemUsingAction*/)
        {
            player.SelectAction(_idleAction);
            _selectedActionType = CombatActionType.Null;
        }
        UpdateButtonSeletable();

        _baseActionBundle.SetActive(false);
        _skillActionBundle.SetActive(false);
        _backgroundImage.SetActive(isDisplayed);
        _skillTooltip.SetActive(false);
        _itemWindow.SetActive(false);
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
                    FieldSystem.unitSystem.GetPlayer().SelectAction(_baseActions[actionType]);
                    SetActionBundle(_activeActionBundle, null);
                    break;
                }
            case CombatActionType.Skills:
                {
                    SetActionBundle(_skillActionBundle, _skillActionBundle);
                    break;
                }
            case CombatActionType.Items:
                {
                    SetItemUI(ItemType.Heal);
                    _itemWindow.SetActive((_selectedActionType != actionType) || (!_itemWindow.activeSelf));
                    break;
                }
            case CombatActionType.Weapons:
                {
                    SetItemUI(ItemType.Revolver);
                    _itemWindow.SetActive((_selectedActionType != actionType) || (!_itemWindow.activeSelf));
                    break;
                }
            case CombatActionType.PlayerSkill:
                {
                    FieldSystem.unitSystem.GetPlayer().SelectAction(_skillActions[btnIdx]);
                    SetActionBundle(_activeActionBundle, null);
                    break;
                }
        }
        _selectedActionType = actionType;
    }

    public void ShowActionUITooltip(GameObject btn)
    {
        if (_activeActionBundle == _skillActionBundle)
        {
            _skillTooltip.GetComponent<CombatActionSkillTooltip>().SetCombatSkillTooltip(btn);
        }
        else
        {
            _buttonNameTooltip.GetComponent<CombatActionNameTooltip>().SetCombatActionTooltip(btn);
        }
    }
    public void HideActionUITooltip() 
    {
        _buttonNameTooltip.GetComponent<CombatActionNameTooltip>().CloseUI();
        _skillTooltip.GetComponent<CombatActionSkillTooltip>().CloseUI();
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
    public bool IsMouseOverActionUI()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        int cnt = 0;
        foreach (var r in results)
        {
            if (r.gameObject.tag == "CombatUI") 
            {
                cnt++;
            }
        }
        if (cnt != 0)
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
        if (_isThereSeletableSkill) return true;

        for (int i = 0; i < _baseActionBundle.transform.childCount; i++) 
        {
            if (i == SKILL_BUTTON_INDEX) continue;
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
        _isThereSeletableSkill = false;
        LoadPlayerAction();
        //skill button activate
        for (int i = 0; i < _skillActionBundle.transform.childCount; i++)
        {
            GameObject btn = _skillActionBundle.transform.GetChild(i).gameObject;
            if (!btn.activeSelf) break;
            btn.GetComponent<CombatActionButtonElement>().SetInteractable();
            if (btn.GetComponent<CombatActionButtonElement>().IsInteractable())
            {
                _isThereSeletableSkill = true;
            }
        }
        bool isSkillExist = _skillActionBundle.transform.GetChild(0).gameObject.activeSelf;

        //item & weapon button activate
        Player player = FieldSystem.unitSystem.GetPlayer();
        bool isEnoughCostForItem = (player.currentActionPoint >= Inventory.ITEM_COST);
        bool isEnoughCostForWeapon = (player.currentActionPoint >= Inventory.WEAPON_COST);

        _baseActionBundle.transform.GetChild(0).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(1).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(2).GetComponent<CombatActionButtonElement>().SetInteractable();
        _baseActionBundle.transform.GetChild(3).GetComponent<CombatActionButtonElement>().SetInteractable(isSkillExist);
        _baseActionBundle.transform.GetChild(4).GetComponent<CombatActionButtonElement>().SetInteractable(isEnoughCostForItem && _itmeUsingAction.IsSelectable());
        _baseActionBundle.transform.GetChild(5).GetComponent<CombatActionButtonElement>().SetInteractable(isEnoughCostForWeapon);
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
    private void SetItemUI(ItemType type)
    {
        //_inventory 불러오기
        Inventory inventory = GameManager.instance.playerInventory;
        if (inventory is null) return;
        List<IItem> items = (List<IItem>)inventory.GetItems(type);
        if (items is null) return;

        for (int i = 0; i < _itemElements.transform.childCount; i++)
        {
            _itemElements.transform.GetChild(i).GetComponent<InventoryUICombatElement>().ClearInventoryUIElement();
        }
        int cnt = 0;
        for (int i = 0; i < items.Count; i++)
        {
            _itemElements.transform.GetChild(cnt++).GetComponent<InventoryUICombatElement>().SetInventoryUIElement((Item)items[i]);
            if (cnt >= _itemElements.transform.childCount) break;
        }

        ClosePopupWindow();
    }
    public void OpenInventoryTooltip(GameObject ui, Vector3 pos)
    {
        _itemTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(ui, pos);
    }
    public override void ClosePopupWindow()
    {
        _itemTooltip.GetComponent<InventoryUITooltip>().CloseUI();
    }
    public int GetInventoryUIIndex(GameObject element)
    {
        for (int i = 0; i < _itemElements.transform.childCount; i++)
        {
            if (_itemElements.transform.GetChild(i).gameObject == element)
            {
                return i;
            }
        }
        Debug.LogError("Can't find inventory ui index");
        return -1;
    }
    public void SeleteUsingItem()
    {
        if (_itemTooltip.activeSelf)
        {
            SetActionBundle(_activeActionBundle, null);
        }
    }
}
