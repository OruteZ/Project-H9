using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatMapInfo", menuName = "ScriptableObjects/CombatMapInfo", order = 1)]
public class CombatMapInfo : ScriptableObject
{
    [SerializeField]
    private List<Vector3Int> enemyPositions;

    public IEnumerable<Vector3Int> GetEnemyPositions()
    {
        return enemyPositions;
    }
    
    public void SetEnemyPositions(IEnumerable<Vector3Int> positions)
    {
        enemyPositions = new List<Vector3Int>(positions);
    }
}