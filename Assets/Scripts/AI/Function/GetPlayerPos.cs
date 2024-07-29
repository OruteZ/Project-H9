using KieranCoppins.DecisionTrees;
using UnityEngine;

public class GetPlayerPos : H9Function<Vector3Int>
{
    public override Vector3Int Invoke()
    {
        return FieldSystem.unitSystem.GetPlayer().hexPosition;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Try to get player position" +
               ". if player is out of sight, " +
               "return current player position";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Try to get player position" +
               ". if player is out of sight, " +
               "return current player position";
    }
}