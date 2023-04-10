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
        _uiManager.OpenCharacterWindow();
    }
    public void OnSkillBtnClick()
    {
        _uiManager.OpenSkillWindow();
    }
    public void OnOptionBtnClick()
    {
        _uiManager.OpenOptionWindow();
    }
}
