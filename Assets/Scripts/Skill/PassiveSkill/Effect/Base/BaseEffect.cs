namespace PassiveSkill
{
    public abstract class BaseEffect : IEffect
    {
        private int _amount;
        private StatType _statType;
        
        private Passive _passive;
        public Unit unit => _passive.unit;
        protected bool enable;

        public BaseEffect(StatType statType, int amount)
        {
            enable = false;
            SetTypeAndAmount(statType, amount);
        }
    
        public void Setup(Passive passive)
        {
            _passive = passive;
            EffectSetup();
        }

        public StatType GetStatType() => _statType;
        public int GetAmount() => _amount;
        protected abstract void EffectSetup();
        public abstract PassiveEffectType GetEffectType();
        public bool IsEnable()
        {
            return enable;
        }

        public abstract void OnConditionEnable();
        public abstract void OnConditionDisable();
        
        
        
        private void SetTypeAndAmount(StatType statType, int amount)
        {
            _amount = amount;
            _statType = statType;
        }

    }    
}