using UnityEngine;

namespace PassiveSkill
{
    public class AmmoIsCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.AmmoIs;

        protected override void ConditionSetup()
        {
            unit.onTurnStart.AddListener((u) => { CheckAmmo(unit.weapon.CurrentAmmo, unit.weapon.CurrentAmmo); });
            unit.onAmmoChanged.AddListener(CheckAmmo);
        }

        private void CheckAmmo(int bef, int aft)
        {
            if(aft == (int)amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
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
            if(aft >= (int)amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
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
            if(aft <= (int)amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }

        public LowAmmoCondition(float amt) : base(amt)
        { }
    }
}