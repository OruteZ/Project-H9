using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private int buttonIndex;
    [SerializeField] private GameObject UiManager;

    public void OnSkillUiButtonClick()
    {
        UiManager.GetComponent<UiManager>().ClickSkillUiButton(this.gameObject, buttonIndex);
    }
}
