using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : Generic.Singleton<SkillUI>
{
    [SerializeField] private int buttonIndex;
    [SerializeField] private UiManager _uiManager;

    //test : 1==½ÀµæºÒ°¡, 2==½Àµæ°¡´É, 3==½Àµæ¿Ï·á
    [SerializeField] private Sprite[] Effect = new Sprite[3];

    [SerializeField] private GameObject[] PrecedenceLine;
    private void Start()
    {
        for (int i = 0; i < PrecedenceLine.Length; i++)
        {
            PrecedenceLine[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void OnSkillUiButtonClick()
    {
        _uiManager.ClickSkillUiButton(this.gameObject.transform, buttonIndex);
    }

    public void SetSkillButtonEffect(int state) 
    {
        this.GetComponent<Image>().sprite = Effect[state];
    }
    public void SetSkillArrow()
    {
        for (int i = 0; i < PrecedenceLine.Length; i++)
        {
            PrecedenceLine[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    public int GetSkillUiIndex() 
    {
        return buttonIndex;
    }
}
