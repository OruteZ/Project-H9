using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private HexTransform _hexTransform;
    private Enemy _enemy;

    private Player player  => CombatSystem.instance.unitSystem.GetPlayer();

    private Vector3Int _playerPosMemory;

    private void Awake()
    {
        _hexTransform = GetComponent<HexTransform>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
    }
    public IUnitAction SelectAction(out Vector3Int targetPosition)
    {
        Tile target;
        if (!IsPlayerInSight())
        {
            if (_enemy.position == _playerPosMemory)
            {
                Debug.LogError("어디를 가라는거야");
            }
            target = CombatSystem.instance.tileSystem.FindPath(_enemy.position, _playerPosMemory)[1];
            targetPosition = target.position;

            return _enemy.GetAction<MoveAction>();
        }
        
        float hitRate = _enemy.weapon.GetHitRate(player);
        if (hitRate <= 0.5f)
        {
            target = CombatSystem.instance.tileSystem.FindPath(_enemy.position, _playerPosMemory)[1];
            targetPosition = target.position;
            return _enemy.GetAction<MoveAction>();
        }

        target = CombatSystem.instance.tileSystem.GetTile(_playerPosMemory);
        targetPosition = target.position;
        return _enemy.GetAction<AttackAction>();
    }

    private bool IsPlayerInSight()
    {
        var curPlayerPos = player.position;
        if (CombatSystem.instance.tileSystem.VisionCast(_enemy.position, curPlayerPos))
        {
            _playerPosMemory = curPlayerPos;
            return true;
        }
        
        return false;
    }
}
