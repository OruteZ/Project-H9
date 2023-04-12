using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public abstract class Unit : MonoBehaviour
{
    private static int _iNextValidID = 0;

    private int _id;

    public int ID
    {
        get => _id;
        set
        {
            _id = value;
            _iNextValidID++;
        }
    }
    
    [HideInInspector]
    public HexTransform hexTransform;

    // ReSharper disable once InconsistentNaming
    public Vector3Int position => hexTransform.position;
    
    public string unitName;
    public virtual void SetUp(string newName)
    {
        ID = _iNextValidID;
        unitName = newName;
        hexTransform = GetComponent<HexTransform>();
    }
    
    public abstract void Updated();
    public abstract void StartTurn();
}