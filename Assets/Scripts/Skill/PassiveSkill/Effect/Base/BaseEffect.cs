namespace PassiveSkill
{
    public abstract class BaseEffect : IEffect
    {
        protected float amount;
        
        private Passive _passive;
        public Unit unit => _passive.unit;

        public BaseEffect(float amount)
        {
            SetAmount(amount);
        }
    
        public void Setup(Passive passive)
        {
            _passive = passive;
            EffectSetup();
        }
        
        private void SetAmount(float amount)
        {
            this.amount = amount;
        }

        protected abstract void EffectSetup();
        public abstract PassiveEffectType GetEffectType();

        public abstract void Enable();
        public abstract void Disable();
    }    
}