using System;
using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;
using Action = KieranCoppins.DecisionTrees.Action;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private H9DecisionTree _tree;
    
    [SerializeField]
    private Unit _unit;
    
    public Vector3Int playerPosMemory;

    public void Awake()
    {
        _tree.Initialise(this);
    }

    public void Start()
    {
        Think();
    }

    public Unit GetUnit()
    {
        return _unit;
    }

    public void Think()
    {
        Debug.Log("run");
        Action result = _tree.Run();
        
        Debug.Log("result : " + result);
        StartCoroutine(result.Execute());
    }
    
    public Vector3Int GetPlayerPosMemory()
    {
        return playerPosMemory;
    }
}

public struct AIResult
{
    //constructor
    public AIResult(IUnitAction action, Vector3Int position)
    {
        this.action = action;
        this.position = position;
    }
    
    public IUnitAction action;
    public Vector3Int position;
}
