using KieranCoppins.DecisionTrees;
using UnityEngine;

public class IsOutOfSightPos : H9Condition
{
    [HideInInspector] [SerializeField] private Function<Vector3Int> position;

    public IsOutOfSightPos(Function<Vector3Int> position)
    {
        this.position = position;
    }
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        position.Initialise(metaData);
    }

    public override DecisionTreeNode GetBranch()
    {
        return Evaluate() ? TrueNode : FalseNode;
    }
    
    private bool Evaluate()
    {
        if (FieldSystem.unitSystem.GetPlayer() is null) return true;
        Unit enemy = ai.GetUnit();
        
        Vector3Int targetPos = position.Invoke();
        if (FieldSystem.tileSystem.VisionCheck(enemy.hexPosition, targetPos) is false)
        {
            return true;
        }
        if (enemy.stat.sightRange < Hex.Distance(enemy.hexPosition, targetPos))
        {
            return true;
        }
             
        return false;
    }
}