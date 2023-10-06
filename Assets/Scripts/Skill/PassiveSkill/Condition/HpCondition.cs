namespace PassiveSkill
{
    public class HpIs : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.HpIs;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after == (int)amount)
                passive.EnableCondition();
            else
                passive.DisableCondition();
        }

        public HpIs(float amt) : base(amt)
        { }
    }
    
    public class HighHp : BaseCondition 
    {
        public override ConditionType GetConditionType() => ConditionType.HighHp;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after >= (int)amount)
                passive.EnableCondition();
            else
                passive.DisableCondition();
        }

        public HighHp(float amt) : base(amt)
        { }
    }
    
    public class LowHp : BaseCondition 
    {
        public override ConditionType GetConditionType() => ConditionType.LowHp;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after <= (int)amount)
                passive.EnableCondition();
            else
                passive.DisableCondition();
        }

        public LowHp(float amt) : base(amt)
        { }
    }
}