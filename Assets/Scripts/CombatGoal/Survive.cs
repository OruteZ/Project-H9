using UnityEngine;
using UnityEngine.Events;

namespace CombatGoal
{
    public class Survive : IGoal
    {
        private UnityAction<bool> _onComplete = null;
        public UnityEvent OnInfoChanged { get; private set; }
        
        private int _turnLimit;
        
        public bool HasSuccess()
        {
            // if player is dead, return false
            if (FieldSystem.unitSystem.GetPlayer().HasDead())
            {
                return false;
            }
            
            int currentTurn = FieldSystem.turnSystem.GetTurnNumber();
            return currentTurn >= _turnLimit;
        }

        public bool IsFinished()
        {
            int currentTurn = FieldSystem.turnSystem.GetTurnNumber();
            
            return currentTurn >= _turnLimit;
        }

        public int GetStringIndex()
        {
            // todo : 뭐 어딘가에는 게임 목표 string이 있겠지?
            return 0;
        }

        public string GetProgressString()
        {
            int currentTurn = FieldSystem.turnSystem.GetTurnNumber();
            
            return $" {currentTurn} / {_turnLimit} ";
        }

        public void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy)
        {
            if (turnLimit != IGoal.INFINITE)
            {
                Debug.LogError("Survive goal does not support infinite turn limit");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                
                return;
            }
            
            _onComplete = null;
            _turnLimit = turnLimit;
            
            // setup event
            OnInfoChanged = new UnityEvent();
            FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
            FieldSystem.unitSystem.GetPlayer().onDead.AddListener(PlayerDead);
        }

        public void AddListenerOnComplete(UnityAction<bool> onComplete)
        {
            _onComplete += onComplete;
        }
        
        #region PRIVATE
        
        private void PlayerDead(Unit none)
        {
            _onComplete?.Invoke(false);
        }
        
        private void OnTurnEnd()
        {
            OnInfoChanged?.Invoke();
            if (IsFinished())
            {
                _onComplete?.Invoke(HasSuccess());
            }
        }
        
        #endregion
    }
}