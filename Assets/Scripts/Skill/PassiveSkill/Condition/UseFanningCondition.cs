using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PassiveSkill
{
    public class UseFanningCondition : BaseCondition
    {
        public UseFanningCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.UseFanning;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartFanning);
            unit.onFinishAction.AddListener(EndFanning);
        }

        protected void StartFanning(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is FanningAction) passive.FullfillCondition(this);
        }

        protected void EndFanning(IUnitAction unitAction)
        {
            if (unitAction is FanningAction) passive.NotFullfillCondition(this);
        }
    }
    public class UseFanningAndCheckChanceCondition : UseFanningCondition
    {
        public UseFanningAndCheckChanceCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType()
        {
            return ConditionType.UseFanningAndCheckChance;
        }
        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartFanning);
            //unit.onFinishAction.AddListener(EndFanning);
        }

        private new void StartFanning(IUnitAction unitAction, Vector3Int pos)
        {
            System.Random rand = new System.Random();
            if (rand.Next(0, 100) >= amount) return;
            base.StartFanning(unitAction, pos);
        }
    }
    public class HitSixFanningShotCondition : BaseCondition
    {
        public HitSixFanningShotCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.HitSixFanningShot;

        private bool _isFanning = false;
        private int _fanningHitCnt = 0;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartFanning);
            unit.onFinishAction.AddListener(EndFanning);
            unit.onFinishShoot.AddListener(CountHit);
        }

        protected void StartFanning(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is FanningAction)
            {
                _isFanning = true;
                _fanningHitCnt = 0;
            }
            else 
            {
                passive.NotFullfillCondition(this);
            }
        }
        protected void CountHit(Damage context)
        {
            if (_isFanning && context.Contains(Damage.Type.MISS) is false)
            {
                _fanningHitCnt++;
            }
        }

        protected void EndFanning(IUnitAction unitAction)
        {
            if (unitAction is FanningAction) 
            {
                _isFanning = false;
                if (_fanningHitCnt >= 6) 
                {
                    passive.FullfillCondition(this);
                }
            }
        }
    }
}
