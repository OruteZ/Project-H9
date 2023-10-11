namespace PassiveSkill
{
    public abstract class BaseEffect : IEffect
    {
        private float _amount;
        private UnitStatType _statType;
        
        private Passive _passive;
        public Unit unit => _passive.unit;
        protected bool enable;

        public BaseEffect(UnitStatType statType, float amount)
        {
            enable = false;
            SetTypeAndAmount(statType, amount);
        }
    
        public void Setup(Passive passive)
        {
            _passive = passive;
            EffectSetup();
        }

        public UnitStatType GetStatType() => _statType;
        public float GetAmount() => _amount;
        protected abstract void EffectSetup();
        public abstract PassiveEffectType GetEffectType();
        public bool IsEnable()
        {
            return enable;
        }

        public abstract void OnConditionEnable();
        public abstract void OnConditionDisable();
        
        
        
        private void SetTypeAndAmount(UnitStatType statType, float amount)
        {
            _amount = amount;
            _statType = statType;
        }

    }    
}