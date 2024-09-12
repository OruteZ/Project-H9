using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMap : MonoBehaviour
{
    [SerializeField]
    private StageStyle _stageStyle;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cam Body"))
        {
            LightingManager.instance.SetStageStyle(_stageStyle);
            
            Debug.Log("Set Stage Style: " + _stageStyle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
