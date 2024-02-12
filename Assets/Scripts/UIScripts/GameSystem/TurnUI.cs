using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 진행한 턴 수를 카운트하고, 전투 중에는 다음에 오는 턴의 주인이 누구인지 순서대로 표시하는 기능을 구현한 클래스
/// </summary>
public class TurnUI : UISystem
{
    [SerializeField] private GameObject _turnText;
    [SerializeField] private GameObject _endTurnButton;
    [SerializeField] private GameObject _endTurnButtonIcon;
    [SerializeField] private GameObject _endTurnButtonEffect;

    [SerializeField] private Sprite _turnOnSprite;
    [SerializeField] private Sprite _turnOffSprite;

    private bool _isEndTurnHighlighted = false;

    private void Awake()
    {
        UIManager.instance.onActionChanged.AddListener(SetEndTurnButton);
        UIManager.instance.onTurnChanged.AddListener(SetEndTurnButton);
        _endTurnButtonEffect.GetComponent<Animator>().enabled = false;
        _endTurnButtonIcon.GetComponent<Image>().sprite = _turnOffSprite;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            OnClickEndTurnButton();
        }
#endif 
    }

    /// <summary>
    /// 화면 우측 상단에 현재 턴 수를 표시한다.
    /// </summary>
    /// <param name="currentTurn"> 현재 턴 수 </param>
    public void SetTurnTextUI() 
    {
        int currentTurn = 0;
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            Player player = FieldSystem.unitSystem.GetPlayer();
            if (player is null) return;
            currentTurn = player.currentRound;
        }
        else 
        {
            currentTurn = FieldSystem.turnSystem.turnNumber;
        }
        _turnText.GetComponent<TextMeshProUGUI>().text = "Turn " + currentTurn;
    }
    public void SetEndTurnButton() 
    {
        _isEndTurnHighlighted = false;
        Color color = UICustomColor.NormalStateColor;
        if (!IsButtonInteractable()) color = UICustomColor.DisableStateColor;
        else if (IsButtonHighlighted())
        {
            _isEndTurnHighlighted = true;
            _endTurnButtonEffect.GetComponent<Animator>().enabled = true;
            _endTurnButtonEffect.GetComponent<Animator>().Play("Inner FadeOut Effect");
            _endTurnButtonIcon.GetComponent<Image>().sprite = _turnOnSprite;
            color = UICustomColor.HighlightStateColor;
        }


        _endTurnButton.GetComponent<Image>().color = color;
        if (!_isEndTurnHighlighted)
        {
            _endTurnButtonEffect.GetComponent<Animator>().Rebind();
            _endTurnButtonEffect.GetComponent<Animator>().enabled = false;
            _endTurnButtonIcon.GetComponent<Image>().sprite = _turnOffSprite;
        } 
    }

    /// <summary>
    /// 턴 종료 버튼을 클릭할 시 실행됩니다.
    /// turnSystem에 플레이어의 턴을 종료하라고 명령을 보냅니다.
    /// </summary>
    public void OnClickEndTurnButton() 
    {
        if (IsButtonInteractable())
        {
            var player = FieldSystem.unitSystem.GetPlayer();
            player.EndTurn();
        }
    }

    private bool IsButtonInteractable()
    {
        if (FieldSystem.turnSystem.turnOwner is not Player) return false;
        if (GameManager.instance.CompareState(GameState.Combat) && FieldSystem.unitSystem.IsCombatFinish(out var none))
            return false;
        if (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive()) return false;
        
        return true;
    }
    private bool IsButtonHighlighted() 
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            return (FieldSystem.unitSystem.GetPlayer().currentActionPoint <= 0);
        }
        else
        {
            return !UIManager.instance.combatUI.combatActionUI_legacy.IsThereSeletableButton();
        }
    }
}
