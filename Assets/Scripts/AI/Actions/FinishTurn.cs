using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class FinishTurn : H9Action, IAiResult
{
    public override IEnumerator Execute()
    {
        Unit unit = ai.GetUnit();
        unit.EndTurn();
        
        yield return null;
    }

    public AIResult GetResult()
    {
        return new AIResult();
    }
}
