using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable InconsistentNaming

public class HexTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _position;

    /// <summary> hex좌표를 의미합니다. 이 값을 변경할 경우 그에 알맞은 transform으로 실제 좌표가 변경됩니다. </summary>
    public Vector3Int position { get => _position; set => SetPosition(value); }

    [ExecuteInEditMode]
    private void SetPosition(Vector3Int pos)
    {
        Transform tsf = transform;
        
        _position = pos;
        tsf.position = Hex.Hex2World(_position) + new Vector3(0, tsf.position.y, 0);
    }

    public void OnValidate()
    {
        SetPosition(_position);
    }
}