using UnityEngine;
using UnityEngine.Events;

namespace CombatGoal
{
    public class MoveToPoint : IGoal
    {
        public UnityEvent OnInfoChanged { get; private set; }
        
        private UnityAction<bool> _onComplete = null;
        private Player _player;
        
        private Vector3Int _targetPos;
        private int _turnLimit;
        
        public bool HasSuccess()
        {
            if (_player.HasDead())
            {
                return false;
            }
            
            return (_player.GetHex() == _targetPos);
        }

        public bool IsFinished()
        {
            if (HasSuccess())
            {
                return true;
            }
            
            // check player dead
            if (_player.HasDead())
            {
                return true;
            }
            
            // check turn limit 
            if (_turnLimit != IGoal.INFINITE)
            {
                if (FieldSystem.turnSystem.GetTurnNumber() >= _turnLimit)
                {
                    return true;
                }
            }
            
            return false;
        }

        public int GetStringIndex()
        {
            return -130;
        }

        public string GetProgressString()
        {
            return $" {_player.GetHex()} / {_targetPos} ";
        }

        public void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy)
        {
            _onComplete = null;
            OnInfoChanged = new UnityEvent();
            
            _player = FieldSystem.unitSystem.GetPlayer();
            _targetPos = targetPos;
            _turnLimit = turnLimit;
            
            // setup event
            FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnUnitMoved);
            FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
            FieldSystem.unitSystem.GetPlayer().onDead.AddListener(PlayerDead);
        }

        public void AddListenerOnComplete(UnityAction<bool> onComplete)
        {
            _onComplete += onComplete;
        }
        
        private void OnTurnEnd()
        {
            if (IsFinished())
            {
                _onComplete?.Invoke(HasSuccess());
            }
        }
        
        private void PlayerDead(Unit none)
        {
            _onComplete?.Invoke(false);
        }
        
        private void OnUnitMoved(Unit unit)
        {
            if (unit is not Player) return;
            
            OnInfoChanged.Invoke();
            if (IsFinished())
            {
                _onComplete?.Invoke(HasSuccess());
            }
        }
    }
}