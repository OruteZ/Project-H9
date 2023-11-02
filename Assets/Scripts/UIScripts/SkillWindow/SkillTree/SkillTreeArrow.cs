using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeArrow : UIElement
{
    [SerializeField] private GameObject _line;
    [SerializeField] private GameObject _fill;

    [SerializeField] private GameObject _preSkill;
    [SerializeField] private GameObject _postSkill;

    public bool isProgressed = false;
    // Start is called before the first frame update
    void Awake()
    {
        _line.GetComponent<Image>().color = UICustomColor.SkillIconNotLearnedColor;
        _fill.GetComponent<Image>().fillAmount = 0;
        isProgressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isProgressed && _fill.GetComponent<Image>().fillAmount < 1) 
        {
            float speed = _postSkill.GetComponent<SkillTreeElement>().effectSpeed;
            _fill.GetComponent<Image>().fillAmount += Time.deltaTime * speed;
        }
    }
    public void SetSkillTreeArrow(GameObject preSkill, GameObject postSkill) 
    {
        if (_preSkill != null || _postSkill != null) return;

        GetComponent<RectTransform>().localPosition = preSkill.GetComponent<RectTransform>().localPosition;
        _preSkill = preSkill;
        _postSkill = postSkill;
    }

    public void ProgressArrow()
    {
        if (!isProgressed)
        {
            _fill.GetComponent<Image>().color = UICustomColor.SkillIconLearnedColor;
            //_fill.GetComponent<Image>().fillAmount = 1;
            isProgressed = true;
        }
    }

    public GameObject GetPreSkillNode()
    {
        return _preSkill;
    }
    public GameObject GetPostSkillNode()
    {
        return _postSkill;
    }


}
