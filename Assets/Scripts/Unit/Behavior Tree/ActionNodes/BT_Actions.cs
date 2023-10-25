using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class FinishTurn : ActionNode
    {
        public FinishTurn(BehaviourTree tree) : base(tree)
        {
        }

        public override INode.ENodeState Evaluate()
        {
            tree.GetUnit().animator.Play("Idle");
            FieldSystem.turnSystem.EndTurn();
            return INode.ENodeState.Success;
        }
    }
    
    public class TryReload : ActionNode
    {
        public TryReload(BehaviourTree tree) : base(tree)
        { }

        public override INode.ENodeState Evaluate()
        {
            var unit = tree.GetUnit();
            
            var action = unit.GetAction<ReloadAction>();
            if (action is null)
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TrySelectAction(action))
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TryExecute(Vector3Int.zero))
            {
                return INode.ENodeState.Failure;
            }

            return INode.ENodeState.Success;
        }
    }

    public class TryMoveOneTileToPlayer : ActionNode
    {
        private List<Tile> _route = null;
        private Vector3Int _destination;
        private int _curIndex;
        
        private bool _hasMapChanged;

        private void OnSetup()
        {
        }
        

        public TryMoveOneTileToPlayer(BehaviourTree tree) : base(tree)
        {
            _hasMapChanged = false;
            FieldSystem.unitSystem.onAnyUnitMoved.AddListener(CheckMapChanged);
        }

        public override INode.ENodeState Evaluate()
        {
            if (IsRouteStillValid() is false)
            {
                if (TryResetRoute() is false)
                {
                    return INode.ENodeState.Failure;
                }
            }
            Unit unit = tree.GetUnit();

            var action = unit.GetAction<MoveAction>();
            if (action is null)
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TrySelectAction(action))
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TryExecute(_route[++_curIndex].hexPosition))
            {
                return INode.ENodeState.Failure;
            }

            return INode.ENodeState.Success;
        }
        private bool TryResetRoute()
        {
            _route = FieldSystem.tileSystem.FindPath(tree.GetUnit().hexPosition, _destination);
            _hasMapChanged = false;

            if (_route is null) return false;
            if (_route.Count <= 1) return false;

            _curIndex = 0;
            return true;
        }
        private bool IsRouteStillValid()
        {
            if (HasTargetChanged()) return false;
            if (HasBeenOffCourse()) return false;
            if (_hasMapChanged) return false;

            return true;
        }
        private bool HasTargetChanged()
        {
            var playerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;
            if (_destination != playerPos)
            {
                _destination = playerPos;
                return true;
            }

            return false;
        }
        private bool HasBeenOffCourse()
        {
            var unit = tree.GetUnit();

            if (unit.hexPosition == _route[_curIndex].hexPosition)
            {
                return false;
            }
            
            //예상 위치는 아니어도 경로 위에 있는지 확인
            for (int i = 0; i < _route.Count; i++)
            {
                if (_route[i].hexPosition == unit.hexPosition)
                {
                    _curIndex = i;
                    return false;
                }
            }

            return true;
        }
        
        private void CheckMapChanged(Unit unit)
        {
            if (_hasMapChanged) return;
            if (unit == tree.GetUnit()) return;
            
            _hasMapChanged = true;
        }
    }

    public class TryShootPlayer : ActionNode
    {
        public TryShootPlayer(BehaviourTree tree) : base(tree)
        {
        }

        public override INode.ENodeState Evaluate()
        {
            Unit unit = tree.GetUnit();
            Player target = FieldSystem.unitSystem.GetPlayer();

            var action = unit.GetAction<AttackAction>();
            if (action is null)
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TrySelectAction(action))
            {
                return INode.ENodeState.Failure;
            }

            if (!((Enemy)unit).TryExecute(target.hexPosition))
            {
                return INode.ENodeState.Failure;
            }

            return INode.ENodeState.Success;
        }
    }
}