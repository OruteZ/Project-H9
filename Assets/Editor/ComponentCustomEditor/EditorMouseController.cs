using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(EditorMouseController))]
    public class Editor_EditorMouseController : Editor
    {
        public override void OnInspectorGUI()
        {
            //button 
            base.OnInspectorGUI();
            
            EditorMouseController script = (EditorMouseController)target;

            if (GUILayout.Button("Clear Command Setting"))
            {
                script.ClearCommand();
            }
        }
    }