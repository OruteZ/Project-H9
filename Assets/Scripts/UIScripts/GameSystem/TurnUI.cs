using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ������ �� ���� ī��Ʈ�ϰ�, ���� �߿��� ������ ���� ���� ������ �������� ������� ǥ���ϴ� ����� ������ Ŭ����
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
        UIManager.instance.onPlayerStatChanged.AddListener(SetEndTurnButton);
        _endTurnButtonEffect.GetComponent<Animator>().enabled = false;
        _endTurnButtonIcon.GetComponent<Image>().sprite = _turnOffSprite;

        _endTurnButton.SetActive(GameManager.instance.CompareState(GameState.COMBAT));
        UIManager.instance.onTSceneChanged.AddListener((gs) => { _endTurnButton.SetActive(gs == GameState.COMBAT); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(HotKey.endTurnKey))
        {
            OnClickEndTurnButton();
        }
    }

    /// <summary>
    /// ȭ�� ���� ��ܿ� ���� �� ���� ǥ���Ѵ�.
    /// </summary>
    /// <param name="currentTurn"> ���� �� �� </param>
    public void SetTurnTextUI() 
    {
        int currentTurn = 0;
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            Player player = FieldSystem.unitSystem.GetPlayer();
            if (player is null) return;
            currentTurn = player.currentRound;
        }
        else 
        {
            currentTurn = FieldSystem.turnSystem.GetTurnNumber();
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
            _endTurnButtonIcon.GetComponent<Image>().color = Color.white;
            color = UICustomColor.HighlightStateColor;
        }


        _endTurnButton.GetComponent<Image>().color = color;
        if (!_isEndTurnHighlighted)
        {
            _endTurnButtonEffect.GetComponent<Animator>().Rebind();
            _endTurnButtonEffect.GetComponent<Animator>().enabled = false;
            _endTurnButtonIcon.GetComponent<Image>().sprite = _turnOffSprite;
            _endTurnButtonIcon.GetComponent<Image>().color = Color.gray;
        } 
    }

    /// <summary>
    /// �� ���� ��ư�� Ŭ���� �� ����˴ϴ�.
    /// turnSystem�� �÷��̾��� ���� �����϶�� ����� �����ϴ�.
    /// </summary>
    public void OnClickEndTurnButton() 
    {
        if (IsButtonInteractable())
        {
            var player = FieldSystem.unitSystem.GetPlayer();
            player.EndTurn();
            UIManager.instance.SetUILayer(1);
        }
    }

    private bool IsButtonInteractable()
    {
        if (FieldSystem.turnSystem.turnOwner is not Player) return false;
        if (GameManager.instance.CompareState(GameState.COMBAT) && FieldSystem.IsCombatFinish(out bool none))
            return false;
        if (FieldSystem.unitSystem.GetPlayer().GetSelectedAction().IsActive()) return false;
        if (GameManager.instance.CompareState(GameState.WORLD) 
            && FieldSystem.unitSystem.GetPlayer().currentActionPoint != 0 
            && FieldSystem.unitSystem.GetPlayer().GetSelectedAction() is not MoveAction) 
            return false;

        return true;
    }
    private bool IsButtonHighlighted() 
    {
        if (GameManager.instance.CompareState(GameState.WORLD))
        {
            return (FieldSystem.unitSystem.GetPlayer().currentActionPoint <= 0);
        }
        else
        {
            return !UIManager.instance.combatUI.combatActionUI.IsThereSeletableButton();
        }
    }
}
