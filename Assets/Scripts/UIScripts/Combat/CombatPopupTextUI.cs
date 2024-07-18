using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 전투 중 화면 중앙에 텍스트로 표시하는 기능을 수행하는 클래스
/// </summary>
public class CombatPopupTextUI : UISystem
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _subText;

    private bool _isTurnTextDisplayed;
    private bool _isActionTextDisplayed;
    private IUnitAction selectedAction = null;
    // Start is called before the first frame update
    void Start()
    {
        ClearText();
        UIManager.instance.onSceneChanged.AddListener(() =>
        {
            selectedAction = null;
            ClearText();
        });
    }

    public void ClearText()
    {
        _mainText.text = "";
        _mainText.enabled = false;

        _subText.text = "";
        _subText.enabled = false;

        _isActionTextDisplayed = false;
    }

    /// <summary>
    /// 현재 시작되는 턴의 주인을 Unit 클래스로 입력받으면 그에 맞는 텍스트를 화면에 출력합니다.
    /// 전투 씬의 턴이 시작될 때마다 실행됩니다.
    /// </summary>
    /// <param name="unit"> 현재 시작되는 턴 주인 </param>
    public void SetStartTurnTextUI(Unit unit) 
    {
        if (!GameManager.instance.CompareState(GameState.Combat)) return;

        StopAllCoroutines();
        if (unit is Player)
        {
            StartCoroutine(ShowTurnText(UIManager.instance.UILocalization[1111], 2.0f));
            
        }
        else
        {
            StartCoroutine(ShowTurnText(UIManager.instance.UILocalization[1112], 2.0f));
        }
    }

    IEnumerator ShowTurnText(string text, float time)
    {
        _isTurnTextDisplayed = true;
        _isActionTextDisplayed = false;

        _mainText.enabled = true;
        _mainText.text = text;
        yield return new WaitForSeconds(time);
        ClearText();
        _isTurnTextDisplayed = false;
    }
    public void SetActionSeleteTextUI(IUnitAction action)
    {
        if (!GameManager.instance.CompareState(GameState.Combat)) return;
        if (FieldSystem.turnSystem.turnOwner is not Player) return;
        if (action is IdleAction) 
        {
            if (!_isTurnTextDisplayed) ClearText();
            return;
        }

        StopAllCoroutines();
        Player player = FieldSystem.unitSystem.GetPlayer();
        string mainText = "";

        if (action is MoveAction)
        {
            mainText = UIManager.instance.UILocalization[1106];
        }
        else if (action is CoverAction)
        {
            mainText = UIManager.instance.UILocalization[1107];
        }
        else if (action is AttackAction)
        {
            mainText = UIManager.instance.UILocalization[1108];
        }
        else if (action is ItemUsingAction)
        {
            string itemName = GameManager.instance.itemDatabase.GetItemScript(((ItemUsingAction)player.GetSelectedAction()).GetItem().GetData().nameIdx).GetName();
            mainText = GetSubstitudeString(UIManager.instance.UILocalization[1110], "<itemName>", itemName);
        }
        else
        {
            string skillName = SkillManager.instance.GetSkillName(player.GetSelectedAction().GetSkillIndex());
            mainText = GetSubstitudeString(UIManager.instance.UILocalization[1109], "<skillName>", skillName);
        }

        StartCoroutine(ShowActionText(mainText));
    }
    private string GetSubstitudeString(string origin, string split, string replace)
    {
        int startIndex = origin.IndexOf(split);
        int endIndex = startIndex + split.Length;
        string afterString = origin.Substring(endIndex, origin.Length - endIndex);

        return replace + afterString;
    }
    IEnumerator ShowActionText(string mText)
    {
        _isActionTextDisplayed = true;
        _isTurnTextDisplayed = false;

        _mainText.enabled = true;
        _mainText.text = mText;
        _subText.enabled = true;
        _subText.text = UIManager.instance.UILocalization[1105];

        while (_isActionTextDisplayed)
        {
            yield return null;
        }
        ClearText();
    }
}
