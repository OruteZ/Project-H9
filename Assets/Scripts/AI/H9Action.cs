using System.Collections;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public abstract class H9Action : Action
{
    protected EnemyAI ai;
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        
        Debug.Log("Initialising action" + this.name);
        ai = (metaData as H9DecisionTree)?.GetAI();
    }

    public override IEnumerator Execute()
    {
        yield break;
    }
}