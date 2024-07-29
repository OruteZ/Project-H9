using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetPlayerMemory : H9Function<Vector3Int>
{
    public override Vector3Int Invoke()
    {
        return ai.playerPosMemory;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Try to get player position" +
               ". if player is out of sight, " +
               "return current player position memory";
    }
}
