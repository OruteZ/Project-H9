using System;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class Distance : H9Function<int>
{
    [SerializeField] [HideInInspector] private Vector3Int posA;
    [SerializeField, HideInInspector] private Vector3Int posB;
    
    //constructor
    public Distance(Vector3Int posA, Vector3Int posB)
    {
        this.posA = posA;
        this.posB = posB;
    }

    public override int Invoke()
    {
        return Hex.Distance(posA, posB);
    }
    
    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get distance to target";
    }
}