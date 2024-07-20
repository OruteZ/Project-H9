using System.Collections;
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

public class FinishTurn : H9Action
{
    public override IEnumerator Execute()
    {
        Unit unit = ai.GetUnit();
        unit.EndTurn();
        
        yield return null;
    }
}
