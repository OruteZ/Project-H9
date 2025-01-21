using System.Collections;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public abstract class H9Action : Action
{
    protected EnemyAI ai;
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        Debug.Log("Initialising action" + name + " with metadata " + metaData);
        ai = (metaData as EnemyAI);
    }

    public override IEnumerator Execute()
    {
        yield break;
    }
}