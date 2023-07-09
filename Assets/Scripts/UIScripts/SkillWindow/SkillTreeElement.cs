using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeElement : Generic.Singleton<SkillTreeElement>
{
    public int skillIndex;
    private UIManager _uiManager;

    //test : 1==½ÀµæºÒ°¡, 2==½Àµæ°¡´É, 3==½Àµæ¿Ï·á
    [SerializeField] private Sprite[] _effect = new Sprite[3];

    [SerializeField] private GameObject[] _precedenceLine;
    private void Start()
    {
        _uiManager = UIManager.instance;
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void OnSkillUIBtnClick()
    {
        _uiManager._skillUI.ClickSkillUIButton(this.gameObject.transform, skillIndex);
    }

    public void SetSkillButtonEffect(int state) 
    {
        this.GetComponent<Image>().sprite = _effect[state];
    }
    public void SetSkillArrow()
    {
        for (int i = 0; i < _precedenceLine.Length; i++)
        {
            _precedenceLine[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    public int GetSkillUIIndex() 
    {
        return skillIndex;
    }
}
