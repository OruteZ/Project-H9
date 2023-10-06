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

    private bool _isInteractable = true;
    private bool _isHighlight = false;

    private void Update()
    {
        SetEndTurnButton();
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
            currentTurn = FieldSystem.unitSystem.GetPlayer().currentRound;
        }
        else 
        {
            currentTurn = FieldSystem.turnSystem.turnNumber;
        }
        _turnText.GetComponent<TextMeshProUGUI>().text = "Turn " + currentTurn;
    }
    public void SetEndTurnButton() 
    {
        Color color = Color.white;
        if (!IsButtonInteractable()) color = Color.black;
        else if (IsButtonHighlighted()) color = Color.yellow;
        _endTurnButton.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// 턴 종료 버튼을 클릭할 시 실행됩니다.
    /// turnSystem에 플레이어의 턴을 종료하라고 명령을 보냅니다.
    /// </summary>
    public void OnClickEndTurnButton() 
    {
        if (IsButtonInteractable())
        {
            FieldSystem.turnSystem.EndTurn();
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
            return !UIManager.instance.combatUI.combatActionUI.IsThereSeletableButton();
        }
    }
}
