using KieranCoppins.DecisionTrees;
using UnityEngine;

public class FindPlayerPos : H9Function<Vector3Int>
{
    public override Vector3Int Invoke()
    {
        ref Vector3Int playerMemory = ref ai.playerPosMemory;
        Vector3Int playerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;

        if (playerMemory == playerPos) return playerMemory;
        if (FieldSystem.tileSystem.VisionCheck(ai.GetUnit().hexPosition, playerPos))
        {
            playerMemory = playerPos;
            return playerMemory;
        }
        if(ai.GetUnit().stat.sightRange < Hex.Distance(ai.GetUnit().hexPosition, playerPos))
        {
            playerMemory = playerPos;
            return playerMemory;
        }

        return Hex.none;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Try to get player position" +
               ". if player is out of sight, " +
               "return current player position memory";
    }
}
