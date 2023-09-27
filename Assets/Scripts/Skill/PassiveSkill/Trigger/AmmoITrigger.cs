namespace PassiveSkill
{
    public class AmmoIs : BaseTrigger
    {
        public override TriggerType GetTriggerType() => TriggerType.AmmoIs;

        protected override void TriggerSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft == (int)amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        public AmmoIs(float amt) : base(amt)
        { }
    }
    
    public class HighAmmo : BaseTrigger
    {
        public override TriggerType GetTriggerType() => TriggerType.HighAmmo;

        protected override void TriggerSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft >= (int)amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        public HighAmmo(float amt) : base(amt)
        { }
    }
    
    public class LowAmmo : BaseTrigger
    {
        public override TriggerType GetTriggerType() => TriggerType.LowAmmo;

        protected override void TriggerSetup()
        {
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft <= (int)amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        public LowAmmo(float amt) : base(amt)
        { }
    }
}