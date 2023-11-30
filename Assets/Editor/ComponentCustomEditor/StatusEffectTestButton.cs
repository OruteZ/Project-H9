using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(StatusEffectTester))]
    public class StatusEffectTestButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StatusEffectTester tester = (StatusEffectTester)target;
        
            if (GUILayout.Button("Effect Player") && EditorApplication.isPlaying)
            {
                tester.Effect();
                //UIManager.instance.onPlayerStatChanged.Invoke();
        }
        
            if (GUILayout.Button("Debug Effect") && EditorApplication.isPlaying)
            {
                //get all status from player
                //print status
                var player = FieldSystem.unitSystem.GetPlayer();
            
                //get all displayable status
                var statusList = player.GetDisplayableEffects();
            
                //print displayable status's stack, duration, name if canDisplay
                foreach (var status in statusList)
                {
                    if (status.CanDisplay() is false) continue;
                
                    Debug.Log($"{status.GetIndex()} : {status.GetStack()}stack : {status.GetDuration()}duration");
                }
            }
        }
    }
