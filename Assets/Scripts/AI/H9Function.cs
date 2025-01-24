using KieranCoppins.DecisionTrees;

public abstract class H9Function<T> : Function<T>
{
    protected EnemyAI ai;
    
    public override void Initialise<T1>(T1 metaData)
    {
        base.Initialise(metaData);
        
        if(metaData is EnemyAI data)
            ai = data;
        else
            throw new System.Exception("MetaData is not EnemyAI");
    }
}