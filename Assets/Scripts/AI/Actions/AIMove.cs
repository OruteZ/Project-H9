using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class AIMove : H9Action, IAiResult
{
    [HideInInspector] [SerializeField] private Function<Vector3Int> targetPos;
    
    public AIMove(Function<Vector3Int> targetPos)
    {
        this.targetPos = targetPos;
    }
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        targetPos.Initialise(metaData);
    }
    
    public override DecisionTreeEditorNodeBase Clone()
    {
        var c = Instantiate(this);
        c.targetPos = (Function<Vector3Int>) targetPos.Clone();
        
        return c;
    }
    
    public override IEnumerator Execute()
    {
        Debug.Log("AI Move");
        yield break;
    }

    public AIResult GetResult()
    {
        Vector3Int startPos = ai.GetUnit().hexPosition;
        Vector3Int endPos = targetPos.Invoke();
        
        var path = FieldSystem.tileSystem.FindPath(startPos, endPos);
        if (path.Count == 0)
        {
            return new AIResult(
                null, 
                ai.GetUnit().hexPosition
                );
        }
        
        return new AIResult(
            ai.GetUnit().GetAction<MoveAction>(), 
            path[1].hexPosition
        );
    }
}
