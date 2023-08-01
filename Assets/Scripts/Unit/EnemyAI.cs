using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private HexTransform _hexTransform;
    private Enemy _enemy;

    private Vector3Int _playerPosMemory;

    public IUnitAction resultAction;
    public Vector3Int resultPosition;
    
    
    [SerializeField] private BehaviorTreeRunner _tree;

    private void Awake()
    {
        _hexTransform = GetComponent<HexTransform>();
        _enemy = GetComponent<Enemy>();
        
        //Create AI
        _tree = new BehaviorTreeRunner(
            new SelectorNode(
                new List<INode>
                {
                    new SequenceNode(
                        new List<INode>
                        {
                            new ActionNode(IsOutOfAmmo),
                            new ActionNode(() => {
                                resultAction = _enemy.GetAction<ReloadAction>();
                                resultPosition = Hex.zero;
                                return INode.ENodeState.Success;
                            })
                        }),
                    new SequenceNode(
                        new List<INode>
                        {
                            new ActionNode(IsPlayerOutOfSight),
                            new ActionNode(() => {
                                resultAction = _enemy.GetAction<MoveAction>();
                                resultPosition = MoveOnePos(_playerPosMemory);
                                return INode.ENodeState.Success;
                            })
                        }),
                    new SequenceNode(
                        new List<INode>
                        {
                            new ActionNode(IsOutOfRange),
                            new ActionNode(() => {
                                resultAction = _enemy.GetAction<MoveAction>();
                                resultPosition = MoveOnePos(_playerPosMemory);
                                return INode.ENodeState.Success;
                            })
                        }),
                    new ActionNode(() =>
                    {
                        resultAction = _enemy.GetAction<AttackAction>();
                        resultPosition = _playerPosMemory;
                        return INode.ENodeState.Success;
                    }),
                    
                    new ActionNode(() =>
                    {
                        resultAction = _enemy.GetAction<IdleAction>();
                        resultPosition = Hex.zero;
                        return INode.ENodeState.Success;
                    })
                }));
    }
    
    public void Start() 
    {
        _playerPosMemory = FieldSystem.unitSystem.GetPlayer().hexPosition;
    }
    
    public void SelectAction()
    {
        _tree.Operate();
    }

    private INode.ENodeState IsPlayerOutOfSight()
    {
        var curPlayerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
        if (FieldSystem.tileSystem.VisionCheck(_enemy.hexPosition, curPlayerPos))
        {
            _playerPosMemory = curPlayerPos;
            return INode.ENodeState.Failure;
        }
        
        return INode.ENodeState.Success;
    }

    private INode.ENodeState IsOutOfAmmo()
    {
        return _enemy.weapon.currentAmmo is 0 ? INode.ENodeState.Success : INode.ENodeState.Failure;
    }

    private INode.ENodeState IsOutOfRange()
    {
        return _enemy.weapon.GetRange() < Hex.Distance(_playerPosMemory, _enemy.hexPosition) ?
            INode.ENodeState.Success : INode.ENodeState.Failure;
    }

    private Vector3Int MoveOnePos(Vector3Int target)
    {
        var route = FieldSystem.tileSystem.FindPath(_enemy.hexPosition, target);
        if (route is null) return _enemy.hexPosition;
        if (route.Count <= 1) return route[0].hexPosition;

        return route[1].hexPosition;
    }
}
