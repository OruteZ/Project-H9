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
        _uiManager.OnOffCharacterCanvas(true);
        _uiManager.OnOffSkillCanvas(false);
        _uiManager.OnOffPauseMenuCanvas(false);
    }
    public void OnSkillBtnClick()
    {
        _uiManager.OnOffCharacterCanvas(false);
        _uiManager.OnOffSkillCanvas(true);
        _uiManager.OnOffPauseMenuCanvas(false);
    }
    public void OnPauseMenuBtnClick()
    {
        _uiManager.OnOffCharacterCanvas(false);
        _uiManager.OnOffSkillCanvas(false);
        _uiManager.OnOffPauseMenuCanvas(true);
    }
    public void OnBackgroundBtnClick()
    {
        _uiManager.OnOffCharacterCanvas(false);
        _uiManager.OnOffSkillCanvas(false);
        _uiManager.OnOffPauseMenuCanvas(false);
    }
    public void OnSkillTooltipCloseBtnClick()
    {
        _uiManager.CloseSkillTooltip();
    }
    public void OnSkillTooltipLearnBtnClick()
    {
        _uiManager.ClickLearnSkill();
    }

    public void OnExitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
