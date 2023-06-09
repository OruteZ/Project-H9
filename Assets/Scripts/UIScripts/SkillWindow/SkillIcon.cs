using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    private UIManager _uiManager;
    private SkillManager _skillManager;
    private int skillIndex;
    public void GetSkillIndex(int index) 
    {
        _uiManager = UIManager.instance;
        _skillManager = SkillManager.instance;

        skillIndex = index;
        //FindIconImage();
    }

    private void FindIconImage() 
    {
        Sprite sprite = Resources.Load("Images/" + _skillManager.GetSkill(skillIndex).skillInfo.iconNumber) as Sprite;
        this.GetComponent<Image>().sprite = sprite;
    }
    public void OnSkillUIButtonOver()
    {
        //_uiManager._skillUI.ClickSkillUIButton(this.gameObject.transform, skillIndex);
    }
}
