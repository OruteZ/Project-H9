using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    public Transform trigger;

    [ContextMenu("Save offset")]
    public void SavePosRot()
    {
        rotationOffset = transform.localRotation;
        positionOffset = transform.localPosition;
    }
    
    public Quaternion rotationOffset;
    public Vector3 positionOffset;

    public void SetPosRot()
    {
        transform.SetLocalPositionAndRotation(positionOffset, rotationOffset); 
    }
}
