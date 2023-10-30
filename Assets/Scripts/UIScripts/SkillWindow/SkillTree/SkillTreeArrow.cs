using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
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
        Vector3 myPos = GetComponent<RectTransform>().localPosition;
        Vector3 myAngle = GetComponent<RectTransform>().localEulerAngles;
        Vector3 mySize = GetComponent<RectTransform>().sizeDelta;
        Vector3 prePos = _preSkill.GetComponent<RectTransform>().localPosition;
        Vector3 postPos = _postSkill.GetComponent<RectTransform>().localPosition;
        //position setting
        if (myPos != prePos) 
        {
            GetComponent<RectTransform>().localPosition = prePos;
        }
        //angle setting
        Vector2 v2 = postPos - prePos;
        float targetAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        if (myAngle.z != targetAngle)
        {
            myAngle.z = targetAngle;
            GetComponent<RectTransform>().localEulerAngles = myAngle;
        }
        //length setting
        float targetLength = v2.magnitude;
        if (mySize.x != targetLength) 
        {
            mySize.x = targetLength;
            GetComponent<RectTransform>().sizeDelta = mySize;
        }


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
