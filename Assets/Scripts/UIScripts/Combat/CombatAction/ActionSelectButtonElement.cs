using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;

/// <summary>
/// 전투 시 플레이어의 행동을 선택할 때 쓰이는 행동선택버튼 각각의 기능을 수행하는 클래스
/// </summary>
public class ActionSelectButtonElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private GameObject _skillImage;
    [SerializeField] private GameObject _highlightEffect;

    [SerializeField] private GameObject _APCostUI;
    [SerializeField] private GameObject _AmmoCostUI;

    [SerializeField] private GameObject _ActionNameUI;

    [SerializeField] private Texture2D _textures; //test Texture. 이동, 공격, 장전, 패닝 순서
    private ActionType[] normalActionType = { ActionType.Move, ActionType.Attack, ActionType.Reload };
    public Sprite[] _sprites;
    public IUnitAction displayedAction { get; private set; }

    private bool _isSelectable;
    private bool _isEnoughCost;
    void Awake()
    {
        //_sprites = Resources.LoadAll<Sprite>("Sprite/" + _textures.name);

        _isSelectable = true;
    }

    /// <summary>
    /// 행동선택버튼을 설정합니다.
    /// CombatActionUI에서 모든 행동선택버튼들을 설정하면서 실행됩니다.
    /// </summary>
    /// <param name="action"> 해당 버튼이 수행할 액션 정보 </param>
    /// <param name="player"> 해당 버튼을 수행할 플레이어 캐릭터 개체 </param>
    public void SetActionSelectButton(IUnitAction action, Player player)
    {
        this.displayedAction = action;

        //Button selectable Setting
        // 플레이어의 턴이 아닌 경우 - 선택 불가
        // 플레이어가 이미 행동을 실행 중인 경우 - 선택 불가
        // 플레이어가 어떤 행동을 선택했는데, 해당 버튼이 선택된 버튼이 아닐 경우 - 선택 불가. 단, Idle(Cancel)버튼은 제외.
        // + SetCostIcons() 메소드에서 Cost가 부족하다고 판단되면 - 선택 불가
        _isSelectable = this.displayedAction.IsSelectable();
        IUnitAction playerSelectedAction = player.GetSelectedAction();
        
        bool isPlayerTurn = FieldSystem.turnSystem.turnOwner is Player;
        bool isPlayerSelectAction = (playerSelectedAction.GetActionType() != ActionType.Idle);
        bool isIdleAction = (action.GetActionType() == ActionType.Idle);
        bool isActiveAction = playerSelectedAction.IsActive();
        if ((!isPlayerTurn) || isActiveAction || (isPlayerSelectAction && !isIdleAction))
        {
            _isSelectable = false;
        }

        _isEnoughCost = true;
        SetCostIcons(player.currentActionPoint, player.weapon.currentAmmo);
        if (!_isEnoughCost)
        {
            _isSelectable = false;
        }
        GetComponent<Button>().interactable = _isSelectable;

        //Button Image Setting
        Sprite spr = GetActionSprite(action.GetActionType());
        if (isIdleAction)
        {
            spr = GetActionSprite(playerSelectedAction.GetActionType());
        }
        _skillImage.GetComponent<Image>().sprite = spr;

        Color skillIconColor = Color.gray;
        if (_isSelectable)
        {
            skillIconColor = Color.white;
        }
        _skillImage.GetComponent<Image>().color = skillIconColor;


        //highlight Setting
        Color hLColor = UICustomColor.NormalStateColor;
        bool isRunOutAmmo = ((action.GetActionType() == ActionType.Reload) && (player.weapon.currentAmmo == 0));
        if (isRunOutAmmo) 
        {
            hLColor = UICustomColor.HighlightStateColor;
        }
        if (!_isSelectable) 
        {
            hLColor = UICustomColor.DisableStateColor;
        }
        _highlightEffect.GetComponent<Image>().color = hLColor;

        //Text Setting
        string actionName = action.GetActionType().ToString();
        if (isIdleAction) 
        {
            actionName = playerSelectedAction.GetActionType().ToString();
        }
        _ActionNameUI.GetComponent<TextMeshProUGUI>().text = actionName;
    }

    /// <summary>
    /// 행동선택버튼을 완전히 비활성화시킵니다.
    /// 상호작용은 물론 코스트나 스킬 이미지도 표시하지 않습니다.
    /// 플레이어가 스킬을 배치하지 않아서 행동창 UI에 빈 공간이 있거나,
    /// 이미 해당 행동을 선택하여 Idle 액션이 해당 행동선택버튼을 대체했거나,
    /// 액션이 실행 중인 경우의 Idle 액션이 해당 상태가 됩니다.
    /// </summary>
    public void OffActionSelectButton()
    {
        displayedAction = null;
        //Button selectable Setting
        _isSelectable = false;
        _isEnoughCost = false;
        GetComponent<Button>().interactable = _isSelectable;

        //Cost Icon Visible Setting
        _APCostUI.SetActive(false);
        _AmmoCostUI.SetActive(false);

        //Button Image Setting
        _skillImage.GetComponent<Image>().sprite = null;
        _skillImage.GetComponent<Image>().color = Color.gray;

        //Text Setting
        _ActionNameUI.GetComponent<TextMeshProUGUI>().text = "";
    }

    private void SetCostIcons(int playerCurrentAp, int playerCurrentAmmo)
    {
        int apCost = displayedAction.GetCost();
        //MoveAction의 GetCost는 이전 턴에 MoveAction이 소모한 AP를 반환하는 상태라 임시방편으로 작성
        //if (action.GetActionType() is ActionType.Move) apCost = 1;
        int ammoCost = displayedAction.GetAmmoCost();

        Color32 ApColor = UICustomColor.ActionAPColor;
        Color32 AmmoColor = UICustomColor.ActionAmmoColor;

        SetEachCostIconUI(_APCostUI, apCost, playerCurrentAp, ApColor);
        SetEachCostIconUI(_AmmoCostUI, ammoCost, playerCurrentAmmo, AmmoColor);
    }
    private void SetEachCostIconUI(GameObject icon, int requiredCost, int currentCost, Color initColor)
    {
        //Cost Icon Visible Setting
        icon.SetActive(true);
        if (requiredCost == 0)
        {
            icon.SetActive(false);
        }

        //Cost Icon Color Setting
        icon.GetComponent<Image>().color = initColor;
        if (currentCost < requiredCost)
        {
            icon.GetComponent<Image>().color = Color.gray;
            _isEnoughCost = false;
        }

        //Cost Icon Text Setting
        icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = requiredCost.ToString();
    }

    /// <summary>
    /// 행동선택버튼을 클릭했을 때 플레이어에게 행동을 수행하라고 명령을 보내는 기능을 수행합니다.
    /// </summary>
    public void OnClickActionSeleteButton() 
    {
        if (!GetComponent<Button>().IsInteractable()) return;
        if (FieldSystem.unitSystem.IsCombatFinish(out var none)) return;
        bool isIdleButton = (displayedAction.GetActionType() == ActionType.Idle);
        bool isActiveSelectedAction = (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive());
        if (isIdleButton && isActiveSelectedAction) return;

        UIManager.instance.combatUI.combatActionUI_legacy.SetSelectedActionButton(gameObject);
        FieldSystem.unitSystem.GetPlayer().SelectAction(displayedAction);
    }

    /// <summary>
    /// 행동선택버튼이 마우스오버되었는지를 감지합니다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (displayedAction is not null)
        {
            UIManager.instance.combatUI.combatActionUI_legacy.ShowActionUITooltip(this.gameObject);
        }
    }

    /// <summary>
    /// 행동선택버튼이 마우스오버 상태에서 벗어났는지를 감지합니다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (displayedAction is not null)
        {
            UIManager.instance.combatUI.combatActionUI_legacy.HideActionUITooltip();
        }
    }


    private Sprite GetActionSprite(ActionType actionType) 
    {
        //normal action
        for (int i = 0; i < normalActionType.Length; i++)
        {
            if (normalActionType[i] == actionType)
            {
                return _sprites[i];
;            }
        }

        //skill action 미구현
        if (actionType == ActionType.Fanning)
        {
            return _sprites[3];
        }
        return null;
    }

    public override bool IsInteractable()
    {
        return _isSelectable;
    }
}