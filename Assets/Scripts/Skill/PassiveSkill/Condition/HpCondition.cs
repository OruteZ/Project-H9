namespace PassiveSkill
{
    public class HpIsCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.HpIs;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after == (int)amount)
                passive.FullfillCondition(this);
            else
                passive.NotFullfillCondition(this);
        }

        public HpIsCondition(float amt) : base(amt)
        { }
    }
    
    public class HighHpCondition : BaseCondition 
    {
        public override ConditionType GetConditionType() => ConditionType.HighHp;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after >= (int)amount)
                passive.FullfillCondition(this);
            else
                passive.NotFullfillCondition(this);
        }

        public HighHpCondition(float amt) : base(amt)
        { }
    }
    
    public class LowHpCondition : BaseCondition 
    {
        public override ConditionType GetConditionType() => ConditionType.LowHp;

        protected override void ConditionSetup()
        {
            unit.onHpChanged.AddListener(CheckHp);
        }

        private void CheckHp(int before, int after)
        {
            if (after <= (int)amount)
                passive.FullfillCondition(this);
            else
                passive.NotFullfillCondition(this);
        }

        public LowHpCondition(float amt) : base(amt)
        { }
    }
}