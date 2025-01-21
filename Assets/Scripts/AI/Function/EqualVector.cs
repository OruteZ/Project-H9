using System;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class EqualVector : H9Function<bool>
{
    [SerializeField, HideInInspector] private Function<Vector3Int> a;
    [SerializeField, HideInInspector] private Function<Vector3Int> b;
    
    public EqualVector(Function<Vector3Int> a, Function<Vector3Int> b)
    {
        this.a = a;
        this.b = b;
    }
    
    public override bool Invoke()
    {
        return a.Invoke().Equals(b.Invoke());
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        a.Initialise(metaData);
        b.Initialise(metaData);
    }

    public override DecisionTreeEditorNodeBase Clone()
    {
        EqualVector clone = Instantiate(this);
        clone.a = (Function<Vector3Int>)a.Clone();
        clone.b = (Function<Vector3Int>)b.Clone();
        return clone;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if two values are equal";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Check if two values are equal";
    }
}