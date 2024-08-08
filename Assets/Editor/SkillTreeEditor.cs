using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillTreeEditor : EditorWindow
{
    private string prefabFolder = "";
    private GameObject passiveSkillButtonPrefab;
    private GameObject activeSkillButtonPrefab;
    private GameObject skillArrowPrefab;

    public Transform skillContents;
    private Transform skillButtons;
    private Transform skillArrows;

    private readonly Vector2 buttonInitPosition = new Vector2(737.5f, -100);
    private const float X_GAP = 150;
    private const float Y_GAP = 200;

    [MenuItem("Tools/SkillTreeEditor")]
    public static void Open()
    {
        GetWindow<SkillTreeEditor>();
    }

    private void OnGUI()
    {
        SerializedObject obj1 = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj1.FindProperty("skillContents"));
        obj1.ApplyModifiedProperties();

        if (skillContents == null)
        {
            EditorGUILayout.HelpBox("Need registed objects", MessageType.Warning);
            return;
        }
        skillButtons = skillContents.transform.Find("SkillUI Buttons");
        skillArrows = skillContents.transform.Find("SkillUI Arrows");
        if (skillButtons == null || skillArrows == null)
        {
            EditorGUILayout.HelpBox("Can't Find Roots of Buttons and Arrows", MessageType.Warning);
            return;
        }
        passiveSkillButtonPrefab = Resources.Load<GameObject>(prefabFolder + "Skill UI Button");
        activeSkillButtonPrefab = Resources.Load<GameObject>(prefabFolder + "Skill UI Button_a");
        skillArrowPrefab = Resources.Load<GameObject>(prefabFolder + "Skill UI Arrow");
        if (passiveSkillButtonPrefab == null || activeSkillButtonPrefab == null || skillArrowPrefab == null)
        {
            EditorGUILayout.HelpBox("Can't Find Prefabs", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical("box");
        DrawButton();
        EditorGUILayout.EndVertical();
    }
    private void DrawButton() 
    {
        if (GUILayout.Button("Create PassiveSkillTreeNode")) 
        {
            CreateSkillTreeNode(true);
        }
        if (GUILayout.Button("Create ActiveSkillTreeNode"))
        {
            CreateSkillTreeNode(false);
        }
    }
    private GameObject InstantiateSkillNode(bool isPassiveSkill)
    {
        GameObject skillButtonPrefab;
        if (isPassiveSkill) skillButtonPrefab = passiveSkillButtonPrefab;
        else skillButtonPrefab = activeSkillButtonPrefab;

        GameObject skillTreeNode = PrefabUtility.InstantiatePrefab(skillButtonPrefab) as GameObject;
        skillTreeNode.name = skillButtonPrefab.name + " (" + skillButtons.childCount + ")";
        skillTreeNode.transform.SetParent(skillButtons, true);
        skillTreeNode.transform.SetLocalPositionAndRotation(buttonInitPosition, Quaternion.identity);
        return skillTreeNode;
    }
    private GameObject InstantiateSkillArrow(int preIndex, int postIndex)
    {
        GameObject skillTreeArrow = PrefabUtility.InstantiatePrefab(skillArrowPrefab) as GameObject;
        skillTreeArrow.name = skillArrowPrefab.name + " (" + preIndex + "-" + postIndex + ")";
        skillTreeArrow.transform.SetParent(skillArrows, false);
        return skillTreeArrow;
    }
    private void CreateSkillTreeNode(bool isPassiveSkill) 
    {
        GameObject skillTreeNode = InstantiateSkillNode(isPassiveSkill);

        if (Selection.activeObject is GameObject) 
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            SkillTreeElement preNode = selectedObject.GetComponent<SkillTreeElement>();
            if (preNode is not null)
            {
                GameObject skillArrow = InstantiateSkillArrow(selectedObject.transform.GetSiblingIndex(), skillTreeNode.transform.GetSiblingIndex());
                skillArrow.GetComponent<SkillTreeArrow>().SetSkillTreeArrow(selectedObject, skillTreeNode);
                Vector3 pos = selectedObject.GetComponent<RectTransform>().localPosition;
                pos.y -= Y_GAP;

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
                if (line.Count != 0)
                {
                    pos.x = line[line.Count - 1].GetComponent<SkillTreeArrow>().GetPostSkillNode().transform.localPosition.x + X_GAP;
                }
                skillTreeNode.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
                preNode.AddPostArrow(skillArrow);
            }
        }

        Selection.activeObject = skillTreeNode.gameObject;
    }
}
