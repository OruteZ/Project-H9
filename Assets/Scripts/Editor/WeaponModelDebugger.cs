using UnityEngine;
using UnityEditor;

    //class weapon model's editor
    [CustomEditor(typeof(WeaponModel))]
    public class WeaponModelDebugger : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            //get weapon model
            WeaponModel weaponModel = (WeaponModel)target;
            
            //if weapon model is null, return
            if (weaponModel == null) return;
            
            //if weapon model is not null, show weaponmodel's isVisible value
            EditorGUILayout.LabelField("isVisible", weaponModel.isVisible.ToString());
        }
}