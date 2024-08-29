namespace PassiveSkill
{
    public abstract class BaseEffect : IEffect
    {
        private int _amount;
        private StatType _statType;
        
        protected Passive passive;
        protected Unit unit => passive.unit;
        protected bool enable;

        public BaseEffect(StatType statType, int amount)
        {
            enable = false;
            SetTypeAndAmount(statType, amount);
        }
    
        public void Setup(Passive passive)
        {
            this.passive = passive;
            OnConditionDisable();
            EffectSetup();
        }

        public StatType GetStatType() => _statType;
        public int GetAmount() => _amount;
        public abstract PassiveEffectType GetEffectType();
        protected abstract void EffectSetup();
        public bool IsEnable()
        {
            return enable;
        }

        public abstract void OnConditionEnable();
        public abstract void OnConditionDisable();
        
        public virtual void OnDelete()
        {
            
        }
        
        private void SetTypeAndAmount(StatType statType, int amount)
        {
            _amount = amount;
            _statType = statType;
        }

    }    
}