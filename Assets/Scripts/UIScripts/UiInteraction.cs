using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInteraction : MonoBehaviour
{

    private UiManager _uiManager;

    private void Awake()
    {
        _uiManager = this.gameObject.GetComponent<UiManager>();
    }
    public void OnCharacterBtnClick() 
    {
        _uiManager.OnOffCharacterWindow();
    }
    public void OnSkillBtnClick()
    {
        _uiManager.OnOffSkillWindow();
    }
    public void OnOptionBtnClick()
    {
        _uiManager.OnOffOptionWindow();
    }

    public void OnSkillTooltipCloseBtnClick()
    {
        _uiManager.CloseSkillTooltip();
    }
}
