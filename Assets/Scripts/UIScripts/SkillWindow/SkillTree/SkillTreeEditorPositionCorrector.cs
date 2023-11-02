using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillTreeEditorPositionCorrector : MonoBehaviour
{
    [SerializeField] private GameObject _skillUIArrows;

    public bool isSkillTreeEditing = false;
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && isSkillTreeEditing)
        {
            for (int i = 0; i < _skillUIArrows.transform.childCount; i++) 
            {
                GameObject arrow = _skillUIArrows.transform.GetChild(i).gameObject;
                RectTransform rect = arrow.GetComponent<RectTransform>();
                SkillTreeArrow function = arrow.GetComponent<SkillTreeArrow>();

                Vector3 myPos = rect.localPosition;
                Vector3 myAngle = rect.localEulerAngles;
                Vector3 mySize = rect.sizeDelta;
                Vector3 prePos = function.GetPreSkillNode().GetComponent<RectTransform>().localPosition;
                Vector3 postPos = function.GetPostSkillNode().GetComponent<RectTransform>().localPosition;
                //position setting
                if (myPos != prePos)
                {
                    arrow.GetComponent<RectTransform>().localPosition = prePos;
                }
                //angle setting
                Vector2 v2 = postPos - prePos;
                float targetAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                if (myAngle.z != targetAngle)
                {
                    myAngle.z = targetAngle;
                    arrow.GetComponent<RectTransform>().localEulerAngles = myAngle;
                }
                //length setting
                float targetLength = v2.magnitude;
                if (mySize.x != targetLength)
                {
                    mySize.x = targetLength;
                    arrow.GetComponent<RectTransform>().sizeDelta = mySize;
                }
            }
            
        }
#endif

    }
}
