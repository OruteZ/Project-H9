namespace PassiveSkill
{
    public class AmmoIs : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.AmmoIs;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft == (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        public AmmoIs(float amt) : base(amt)
        { }
    }
    
    public class HighAmmo : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.HighAmmo;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft >= (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        public HighAmmo(float amt) : base(amt)
        { }
    }
    
    public class LowAmmo : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.LowAmmo;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft <= (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        public LowAmmo(float amt) : base(amt)
        { }
    }
}