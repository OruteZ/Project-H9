namespace PassiveSkill
{
    public class AmmoIsCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.AmmoIs;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft == (int)amount) passive.Enable();
            else passive.Disable();
        }

        public AmmoIsCondition(float amt) : base(amt)
        { }
    }
    
    public class HighAmmoCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.HighAmmo;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft >= (int)amount) passive.Enable();
            else passive.Disable();
        }

        public HighAmmoCondition(float amt) : base(amt)
        { }
    }
    
    public class LowAmmoCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.LowAmmo;

        protected override void ConditionSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft <= (int)amount) passive.Enable();
            else passive.Disable();
        }

        public LowAmmoCondition(float amt) : base(amt)
        { }
    }
}