using KieranCoppins.DecisionTrees;
using UnityEngine;

public class OutOfSight : H9Function<bool>
{
    [SerializeField]
    private ActionType actionType;
    
    [HideInInspector] 
    [SerializeField] 
    private Function<Vector3Int> position;
    
    public OutOfSight(Function<Vector3Int> position)
    {
        this.position = position;
    }
    
    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        position.Initialise(metaData);
    }
    
    public override DecisionTreeEditorNodeBase Clone()
    {
        var c = Instantiate(this);
        c.position = (Function<Vector3Int>) position.Clone();
        
        return c;
    }
    
    private bool Evaluate()
    {
        if (FieldSystem.unitSystem.GetPlayer() is null) return true;
        Unit enemy = ai.GetUnit();
        
        Vector3Int targetPos = position.Invoke();
        if (FieldSystem.tileSystem.VisionCheck(enemy.hexPosition, targetPos, lookInside:true) is false)
        {
            return true;
        }
        if (enemy.stat.sightRange < Hex.Distance(enemy.hexPosition, targetPos))
        {
            return true;
        }
             
        return false;
    }

    public override bool Invoke()
    {
        return Evaluate();
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "Check if target is out of sight";
    }
    
    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Check if target is out of sight";
    }
}