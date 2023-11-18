using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitModel))]
public class UnitModelTransformSetter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //get unit model
        UnitModel unitModel = (UnitModel)target;
        
        //set transform button
        if (GUILayout.Button("Set Transform"))
        {
            unitModel.SetTransform();
        }
        
        //remove btn
        if (GUILayout.Button("Remove Transform"))
        {
            unitModel.RemoveDisabled();
        }
    }
}