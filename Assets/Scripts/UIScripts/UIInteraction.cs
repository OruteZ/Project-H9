using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//원래 이 클래스를 이용해서 UI 입력을 처리할 다른 계획이 있었으나 폐기되어서 사실상 필요없는 코드입니다.
//그나마 UIManager가 하는 일이 워낙 많으니 UIManager입력만 따로 처리하는 용도? 정도로만 쓸 수 있을 듯.
//차근차근 함수들을 다른 위치로 이관하고 삭제할 예정.
/// <summary>
/// 각종 UI 상호작용을 처리하는 클래스
/// </summary>
public class UIInteraction : Generic.Singleton<UIInteraction>
{

    private static UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnCharacterBtnClick();
        }
    }
    public void OnCharacterBtnClick()
    {
        _uiManager.SetCharacterCanvasState(true);
        _uiManager.SetSkillCanvasState(false);
        _uiManager.SetPauseMenuCanvasState(false);
    }
    public void OnSkillBtnClick()
    {
        _uiManager.SetCharacterCanvasState(false);
        _uiManager.SetSkillCanvasState(true);
        _uiManager.SetPauseMenuCanvasState(false);
    }
    public void OnPauseMenuBtnClick()
    {
        _uiManager.SetCharacterCanvasState(false);
        _uiManager.SetSkillCanvasState(false);
        _uiManager.SetPauseMenuCanvasState(true);
    }
    public void OnBackgroundBtnClick()
    {
        _uiManager.SetCharacterCanvasState(false);
        _uiManager.SetSkillCanvasState(false);
        _uiManager.SetPauseMenuCanvasState(false);
    }
    public void OnSkillTooltipCloseBtnClick()
    {
        _uiManager.skillUI.CloseSkillTooltip();
    }
    public void OnSkillTooltipLearnBtnClick()
    {
        _uiManager.skillUI.ClickLearnSkill();
    }

    public void OnExitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnWeaponItemUIBtnClick()
    {
        _uiManager.characterUI.itemListUI.ChangeItemUIStatus(ItemInfo.ItemCategory.Weapon);
    }
    public void OnUsableItemUIBtnClick()
    {
        _uiManager.characterUI.itemListUI.ChangeItemUIStatus(ItemInfo.ItemCategory.Usable);
    }
    public void OnOtherItemUIBtnClick()
    {
        _uiManager.characterUI.itemListUI.ChangeItemUIStatus(ItemInfo.ItemCategory.Other);
    }
}
