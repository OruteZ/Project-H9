using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    public Transform trigger;

    [ContextMenu("Save offset : on hand")]
    public void SaveHandPosRot()
    {
        handRotationOffset = transform.localRotation;
        handPositionOffset = transform.localPosition;
    }
    
    [ContextMenu("Save offset : on stand")]
    public void SaveStandPosRot()
    {
        standRotationOffset = transform.localRotation;
        standPositionOffset = transform.localPosition;
    }
    
    public Quaternion handRotationOffset;
    public Vector3 handPositionOffset;

    public Quaternion standRotationOffset;
    public Vector3 standPositionOffset;

    public void SetHandPosRot()
    {
        transform.SetLocalPositionAndRotation(handPositionOffset, handRotationOffset); 
    }

    public void SetStandPosRot()
    {
        transform.SetLocalPositionAndRotation(standPositionOffset, standRotationOffset); 
    }
}
