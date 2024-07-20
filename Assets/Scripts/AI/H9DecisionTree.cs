using KieranCoppins.DecisionTrees;
using UnityEngine;

[CreateAssetMenu(fileName = "H9DecisionTree", menuName = "H9DecisionTree")]
public class H9DecisionTree : DecisionTree
{
    [SerializeField]
    private EnemyAI _ai;
    
    public void Initialise(EnemyAI ai)
    {
        base.Initialise(ai);
        _ai = ai;
    }
    
    public EnemyAI GetAI()
    {
        return _ai;
    }
}