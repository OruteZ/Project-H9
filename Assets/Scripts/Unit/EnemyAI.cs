using System;
using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private DecisionTree _tree;
    
    [SerializeField]
    private Unit _unit;

    public void Start()
    {
        _tree.Run();
    }

    public Unit GetUnit()
    {
        return _unit;
    }
}

public struct AIResult
{
    public IUnitAction action;
    public Vector3Int position;
}
