namespace PassiveSkill
{
    public class TargetHighHp : BaseTrigger 
    {
        public override TriggerType GetTriggerType()
        {
            return TriggerType.TargetHighHp;
        }

        protected override void TriggerSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public TargetHighHp(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp >= amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.TurnOffPassive();
        }
    }
    
    public class TargetHpIs : BaseTrigger
    {
        public override TriggerType GetTriggerType()
        {
            return TriggerType.TargetHpIs;
        }

        protected override void TriggerSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public TargetHpIs(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp == (int)amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.TurnOffPassive();
        }
    }
    
    public class TargetLowHp : BaseTrigger
    {
        public TargetLowHp(float amt) : base(amt)
        { }
        
        public override TriggerType GetTriggerType()
        {
            return TriggerType.TargetLowHp;
        }

        protected override void TriggerSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        private void SetTarget(Unit target)
        {
            if(target.hp <= amount) passive.TurnOnPassive();
            else passive.TurnOffPassive();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.TurnOffPassive();
        }
    }
}