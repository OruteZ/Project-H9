using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private HexTransform _hexTransform;
    private Enemy _enemy;

    private Vector3Int _playerPosMemory;

    private void Awake()
    {
        _hexTransform = GetComponent<HexTransform>();
        _enemy = GetComponent<Enemy>();
    }
    
    public IUnitAction SelectAction(out Vector3Int targetPosition)
    {
        targetPosition = Hex.zero;
        return null;
    }

    private bool IsPlayerInSight()
    {
        var curPlayerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
        if (FieldSystem.tileSystem.VisionCheck(_enemy.hexPosition, curPlayerPos))
        {
            _playerPosMemory = curPlayerPos;
            return true;
        }
        
        return false;
    }
}
