using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnvObject : MonoBehaviour
{
    private HexTransform _hexTransform;

    private HexTransform hexTransform
    {
        get
        {
            if (_hexTransform is null) _hexTransform = GetComponent<HexTransform>();
            return _hexTransform;
        }
    }

    public Vector3Int hexPosition
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    } 
    public abstract void OnSetting();
    
    //-------

    protected void RotateRandom()
    {
        transform.localRotation = Quaternion.Euler(0, Random.value * 360, 0);
    }
}
