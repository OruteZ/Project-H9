using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HexTransform : MonoBehaviour
{
    private Vector3Int _position;
    // ReSharper disable once InconsistentNaming
    public Vector3Int position { get => _position; set => SetPosition(value); }

    private void SetPosition(Vector3Int pos)
    {
        _position = pos;
        transform.position = Hex.Hex2World(_position);
    }

    public void OnValidate()
    {
        transform.position = Hex.Hex2World(_position);
    }
}