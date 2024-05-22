using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [Header("Position refernce")]
    [SerializeField]
    [Tooltip("FX를 보여줄 총구의 위치")]
    private Transform Gunpoint;
    private bool _isExistGunpoint = false;

    [Space()]
    public Transform trigger;
    
    public Quaternion rotationOffset;
    public Vector3 positionOffset;

    private List<MeshRenderer> _meshRendererList;

    public void Awake()
    {
        if (Gunpoint != null)
        {
            _isExistGunpoint = true;
        }

        _meshRendererList = GetComponentsInChildren<MeshRenderer>().ToList();
        _meshRendererList.Add(GetComponent<MeshRenderer>());
    }

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
    
    public Quaternion NOTFRONTIER_handRotationOffset;
    public Vector3 NOTFRONTIER_handPositionOffset;

    public Quaternion standRotationOffset;
    public Vector3 standPositionOffset;
    
    

    public void SetHandPosRot(bool isWesternFrontier = true)
    {
        if (isWesternFrontier)
        {
            transform.SetLocalPositionAndRotation(handPositionOffset, handRotationOffset);
            //set model scale to 100
            transform.localScale = Vector3.one * 100f;
        }
        else
        {
            transform.SetLocalPositionAndRotation(NOTFRONTIER_handPositionOffset, NOTFRONTIER_handRotationOffset);
            //set model scale to 1
            transform.localScale = Vector3.one;
        }
    }
    
    public void SetWaistPosRot(bool isWesternFrontier = true)
    {
        if (isWesternFrontier)
        {
            transform.SetLocalPositionAndRotation(standPositionOffset, standRotationOffset);
            //set model scale to 100
            transform.localScale = Vector3.one * 100f;
        }
        else
        {
            transform.SetLocalPositionAndRotation(NOTFRONTIER_handPositionOffset, NOTFRONTIER_handRotationOffset);
            //set model scale to 1
            transform.localScale = Vector3.one;
        }
    }
    
    public Vector3 GetGunpointPosition()
    {
        if (_isExistGunpoint)
            return Gunpoint.position;
        return transform.position; // 총의 좌표 그대로 반환
    }

    public bool isVisible
    {
        get
        {
            //null check : return false
            if (_meshRendererList is null)
                return false;
            if (_meshRendererList.Count == 0)
                return false;
            return _meshRendererList[0] && _meshRendererList[0].enabled;
        }
        set
        {
            foreach (var r in _meshRendererList)
            {
                r.enabled = value;
            }
        }
    }
}
