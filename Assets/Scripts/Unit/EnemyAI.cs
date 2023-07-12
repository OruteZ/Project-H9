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
        Tile target;
        if (!IsPlayerInSight())
        {
            if (_enemy.hexPosition == _playerPosMemory)
            {
                Debug.LogError("어디를 가라는거야");
            }
            target = FieldSystem.tileSystem.FindPath(_enemy.hexPosition, _playerPosMemory)[1];
            targetPosition = target.hexPosition;

            return _enemy.GetAction<MoveAction>();
        }
        
        float hitRate = _enemy.weapon.GetHitRate(FieldSystem.unitSystem.GetPlayer());
        if (hitRate <= 0.5f)
        {
            target = FieldSystem.tileSystem.FindPath(_enemy.hexPosition, _playerPosMemory)[1];
            targetPosition = target.hexPosition;
            return _enemy.GetAction<MoveAction>();
        }

        target = FieldSystem.tileSystem.GetTile(_playerPosMemory);
        targetPosition = target.hexPosition;
        return _enemy.GetAction<AttackAction>();
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
