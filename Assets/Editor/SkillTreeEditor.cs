using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillTreeEditor : EditorWindow
{
    public GameObject skillButtonPrefab;
    public GameObject skillArrowPrefab;

    public Transform skillButtons;
    public Transform skillArrows;

    [MenuItem("Tools/SkillTreeEditor")]
    public static void Open()
    {
        GetWindow<SkillTreeEditor>();
    }

    private void OnGUI()
    {
        SerializedObject obj1 = new SerializedObject(this);
        SerializedObject obj2 = new SerializedObject(this);
        SerializedObject obj3 = new SerializedObject(this);
        SerializedObject obj4 = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj1.FindProperty("skillButtonPrefab"));
        EditorGUILayout.PropertyField(obj2.FindProperty("skillArrowPrefab"));
        EditorGUILayout.PropertyField(obj3.FindProperty("skillButtons"));
        EditorGUILayout.PropertyField(obj4.FindProperty("skillArrows"));
        if (skillButtonPrefab == null || skillArrowPrefab == null || skillButtons == null || skillArrows == null)
        {
            EditorGUILayout.HelpBox("오브젝트 등록이 필요합니다.", MessageType.Warning);
        }
        else 
        {
            EditorGUILayout.BeginVertical("box");
            DrawButton();
            EditorGUILayout.EndVertical();
        }
        obj1.ApplyModifiedProperties();
        obj2.ApplyModifiedProperties();
        obj3.ApplyModifiedProperties();
        obj4.ApplyModifiedProperties();
    }
    private void DrawButton() 
    {
        if (GUILayout.Button("Create SkillTreeNode")) 
        {
            CreateSkillTreeNode();
        }
    }
    private GameObject InstantiateSkillNode()
    {
        GameObject skillTreeNode = PrefabUtility.InstantiatePrefab(skillButtonPrefab) as GameObject;
        skillTreeNode.name = skillButtonPrefab.name + " (" + skillButtons.childCount + ")";
        skillTreeNode.transform.SetParent(skillButtons, false);
        return skillTreeNode;
    }
    private GameObject InstantiateSkillArrow()
    {
        GameObject skillTreeArrow = PrefabUtility.InstantiatePrefab(skillArrowPrefab) as GameObject;
        skillTreeArrow.name = skillArrowPrefab.name + " (" + skillArrows.childCount + ")";
        skillTreeArrow.transform.SetParent(skillArrows, false);
        return skillTreeArrow;
    }
    private void CreateSkillTreeNode() 
    {
        GameObject skillTreeNode = InstantiateSkillNode();

        if (Selection.activeObject is GameObject) 
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            SkillTreeElement preNode = selectedObject.GetComponent<SkillTreeElement>();
            if (preNode is not null) 
            {
                GameObject skillArrow = InstantiateSkillArrow();
                skillArrow.GetComponent<SkillTreeArrow>().SetSkillTreeArrow(selectedObject, skillTreeNode);
                Vector3 pos = selectedObject.GetComponent<RectTransform>().localPosition;
                pos.x += 200;

                List<GameObject> line = preNode.GetPostArrow();
                for (int i = line.Count - 1; i >= 0; i--)
                {
                    Debug.Log(i + " : " + line[i]);
                    if (line[i] == null)
                    {
                        line.RemoveAt(i);
                        Debug.Log(line.Count);
                    }
                }
                pos.y -= 150 * line.Count;
                skillTreeNode.GetComponent<RectTransform>().localPosition = pos;
                preNode.AddPostArrow(skillArrow);
            }
        }

        Selection.activeObject = skillTreeNode.gameObject;
    }
}
