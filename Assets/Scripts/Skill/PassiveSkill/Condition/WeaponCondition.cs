using UnityEngine;

namespace PassiveSkill
{
    public class EquipRevolverCondition : BaseCondition
    {
        public EquipRevolverCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.EquipRevolver;

        protected override void ConditionSetup()
        {
            unit.onWeaponChange.AddListener(CheckCondition);
        }

        protected void CheckCondition(Weapon weapon)
        {
            if (weapon is Revolver) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
    }
    public class EquipRepeaterCondition : BaseCondition
    {
        public EquipRepeaterCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.EquipRepeater;

        protected override void ConditionSetup()
        {
            unit.onWeaponChange.AddListener(CheckCondition);
        }

        protected void CheckCondition(Weapon weapon)
        {
            if (weapon is Repeater) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
    }
    public class EquipShotgunCondition : BaseCondition
    {
        public EquipShotgunCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.EquipShotgun;

        protected override void ConditionSetup()
        {
            unit.onWeaponChange.AddListener(CheckCondition);
        }

        protected void CheckCondition(Weapon weapon)
        {
            if (weapon is Shotgun) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
    }

}