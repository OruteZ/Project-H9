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

    public string unitName;
    public virtual void Setup(string newName)
    {
        ID = _iNextValidID;
        unitName = newName;
    }
    
    public abstract void Updated();
    public abstract void StartTurn();
}