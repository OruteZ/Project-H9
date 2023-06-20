using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private SkillManager _skillManager;
    private int skillIndex;
    public void GetSkillIndex(int index) 
    {
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
