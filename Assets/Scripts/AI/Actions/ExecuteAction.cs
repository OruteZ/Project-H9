using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class ExecuteAction : H9Action, IAiResult
{
    [HideInInspector] [SerializeField] private Function<IUnitAction> action;
    [SerializeField, HideInInspector] private Function<Vector3Int> targetPos;

    public ExecuteAction(Function<IUnitAction> action, Function<Vector3Int> targetPos)
    {
        this.action = action;
        this.targetPos = targetPos;
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        action.Initialise(metaData);

        if (targetPos != null)
        {
            targetPos.Initialise(metaData);
        }
    }
    
    public override DecisionTreeEditorNodeBase Clone()
    {
        var c = Instantiate(this);
        c.action = (Function<IUnitAction>) action.Clone();
        c.targetPos = targetPos != null ? (Function<Vector3Int>) targetPos.Clone() : null;
        
        return c;
    }

    public override IEnumerator Execute()
    {
        
        yield break;
    }
    
    public AIResult GetResult()
    {
        return new AIResult(
            action.Invoke(), 
            targetPos != null ? targetPos.Invoke() : Hex.none
            );
    }
}