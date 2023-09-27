namespace PassiveSkill
{
    public class HpIs : BaseTrigger
    {
        public override TriggerType GetTriggerType() => TriggerType.HpIs;

        protected override void TriggerSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after == (int)amount)
                passive.TurnOnPassive();
            else
                passive.TurnOffPassive();
        }

        public HpIs(float amt) : base(amt)
        { }
    }
    
    public class HighHp : BaseTrigger 
    {
        public override TriggerType GetTriggerType() => TriggerType.HighHp;

        protected override void TriggerSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after >= (int)amount)
                passive.TurnOnPassive();
            else
                passive.TurnOffPassive();
        }

        public HighHp(float amt) : base(amt)
        { }
    }
    
    public class LowHp : BaseTrigger 
    {
        public override TriggerType GetTriggerType() => TriggerType.LowHp;

        protected override void TriggerSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after <= (int)amount)
                passive.TurnOnPassive();
            else
                passive.TurnOffPassive();
        }

        public LowHp(float amt) : base(amt)
        { }
    }
}