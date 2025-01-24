using System;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class Distance : H9Function<int>
{
    [SerializeField, HideInInspector] private Function<Vector3Int> posA;
    [SerializeField, HideInInspector] private Function<Vector3Int> posB;
    
    //constructor
    public Distance(Function<Vector3Int> posA, Function<Vector3Int> posB)
    {
        this.posA = posA;
        this.posB = posB;
    }

    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        posA.Initialise(metaData);
        posB.Initialise(metaData);
    }

    public override DecisionTreeEditorNodeBase Clone()
    {
        var c = Instantiate(this);
        c.posA = (Function<Vector3Int>) posA.Clone();
        c.posB = (Function<Vector3Int>) posB.Clone();
        
        return c;
    }

    public override int Invoke()
    {
        return Hex.Distance(posA.Invoke(), posB.Invoke());
    }
    
    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Get distance to target";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Get distance to target";
    }
}