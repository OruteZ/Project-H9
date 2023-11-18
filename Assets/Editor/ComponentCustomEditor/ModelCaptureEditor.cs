
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelCapture))]
public class ModelCaptureEditor : Editor
{
    //override inspector gui
    public override void OnInspectorGUI()
    {
        //base inspector gui
        base.OnInspectorGUI();

        //get model capture
        ModelCapture modelCapture = (ModelCapture)target;
        
        ref string objPath = ref modelCapture.objPath;
        ref string pngPath = ref modelCapture.pngPath;

        //if model capture is null, return
        if (modelCapture == null) return;
        
        //set model path with browse button
        if (GUILayout.Button("Set Model Path"))
        {
            objPath = EditorUtility.OpenFolderPanel("Select Model path", "", "");
            
            //replace Assets/Resources to empty string
            objPath = objPath.Replace(Application.dataPath, "Assets");
            objPath = objPath.Replace("Assets/Resources/", "");
        }   
        
        //set png path with browse button
        if (GUILayout.Button("Set PNG Path"))
        {
            pngPath = EditorUtility.OpenFolderPanel("Select PNG Folder", "", "");
        }
        
        //capture button
        if (GUILayout.Button("Capture"))
        {
            modelCapture.Capture();
        }
    }
}