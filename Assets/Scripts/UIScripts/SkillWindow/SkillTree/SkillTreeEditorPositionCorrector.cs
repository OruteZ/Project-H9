using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillTreeEditorPositionCorrector : MonoBehaviour
{
    [SerializeField] private GameObject _skillUIArrows;
    [SerializeField] private GameObject _skillUIButtons;

    public bool isSkillTreeEditing = false;
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && isSkillTreeEditing)
        {
            //buttons
            float minX = 99999;
            float maxX = -99999;
            float minY = 0;
            for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
            {
                Transform obj = _skillUIButtons.transform.GetChild(i);
                if (obj.localPosition.x < minX) minX = obj.localPosition.x;
                if (obj.localPosition.x > maxX) maxX = obj.localPosition.x;
                if (obj.localPosition.y < minY) minY = obj.localPosition.y;
            }
            float middle = (maxX + minX) / 2;
            for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
            {
                Transform obj = _skillUIButtons.transform.GetChild(i);
                obj.localPosition += new Vector3(-middle, 0, 0);
            }



            //Arrows
            List<GameObject> destroyArrows = new();
            for (int i = 0; i < _skillUIArrows.transform.childCount; i++)
            {
                GameObject arrow = _skillUIArrows.transform.GetChild(i).gameObject;
                RectTransform rect = arrow.GetComponent<RectTransform>();
                SkillTreeArrow function = arrow.GetComponent<SkillTreeArrow>();

                if (function.GetPreSkillNode() == null || function.GetPostSkillNode() == null)
                {
                    destroyArrows.Add(arrow);
                    continue;
                }

                Vector3 myPos = rect.position;
                Vector3 myAngle = rect.eulerAngles;
                Vector3 mySize = rect.sizeDelta;
                Vector3 prePos = function.GetPreSkillNode().GetComponent<RectTransform>().position;
                Vector3 postPos = function.GetPostSkillNode().GetComponent<RectTransform>().position;
                //position setting
                if (myPos != prePos)
                {
                    arrow.GetComponent<RectTransform>().position = prePos;
                }
                //angle setting
                Vector2 v2 = postPos - prePos;
                float targetAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                if (myAngle.z != targetAngle)
                {
                    myAngle.z = targetAngle;
                    arrow.GetComponent<RectTransform>().eulerAngles = myAngle;
                }
                //length setting
                float targetLength = v2.magnitude;
                if (mySize.x != targetLength)
                {
                    mySize.x = targetLength;
                    arrow.GetComponent<RectTransform>().sizeDelta = mySize;
                }
            }

            for (int i = 0; i < destroyArrows.Count; i++)
            {
                GameObject post = destroyArrows[i].GetComponent<SkillTreeArrow>().GetPreSkillNode();
                if (post != null) post.GetComponent<SkillTreeElement>().DeletePostArrow(destroyArrows[i]);
                DestroyImmediate(destroyArrows[i]);
            }


            Vector2 size = _skillUIArrows.GetComponent<RectTransform>().sizeDelta;
            size.y = -minY + 100;
            _skillUIArrows.GetComponent<RectTransform>().sizeDelta = size;
            _skillUIButtons.GetComponent<RectTransform>().sizeDelta = size;
            GetComponent<RectTransform>().sizeDelta = size;
        }
#endif
    }
}
