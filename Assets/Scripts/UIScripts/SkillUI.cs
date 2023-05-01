using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private int buttonIndex;
    [SerializeField] private GameObject UiManager;

    //test : 1==½ÀµæºÒ°¡, 2==½Àµæ°¡´É, 3==½Àµæ¿Ï·á
    [SerializeField] private Sprite[] Effect = new Sprite[3];

    public void OnSkillUiButtonClick()
    {
        UiManager.GetComponent<UiManager>().ClickSkillUiButton(this.gameObject.transform, buttonIndex);
    }

    public void SetSkillButtonEffect(int state) 
    {
        this.GetComponent<Image>().sprite = Effect[state];
    }
    public int GetIndex() 
    {
        return buttonIndex;
    }
}
