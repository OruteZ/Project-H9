using UnityEngine;

namespace Generic
{
    public abstract class State<T> where T : class
    {
        public abstract void Enter(T entity);
        public abstract void Execute(T entity);
        public abstract void Exit(T entity);
    }
    
    public class StateMachine<T> where T : class
    {
        private T        _ownerEntity;
        private State<T> _currentState;

        public void SetUp(T owner, State<T> entryState)
        {
            _ownerEntity = owner;
            _currentState = null;

            ChangeState(entryState);

            string value = "1234";
            switch (value)
            {
                case "1234" :
                    Debug.Log("됨");
                    break;
                
            }
        }

        public void Execute()
        {
            _currentState?.Execute(_ownerEntity);
        }

        public void ChangeState(State<T> newState)
        {
            if (newState == null) return;
            _currentState?.Exit(_ownerEntity);
            _currentState = newState;
            newState.Enter(_ownerEntity);
        }
    }
}