using KieranCoppins.DecisionTrees;
using UnityEngine;

public abstract class H9Condition : Decision
{
    protected EnemyAI ai;

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        ai = (metaData as EnemyAI);
    }
}