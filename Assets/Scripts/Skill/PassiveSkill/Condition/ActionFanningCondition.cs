using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedFanningThisTurnCondition : ActionThisTurnCondition<FanningAction>
    {
        public UsedFanningThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedFanningThisTurn;
    }
    public class UsingFanningCondition : ActingCondition<FanningAction>
    {
        public UsingFanningCondition(float amt) : base(amt) {}
        public override ConditionType GetConditionType() => ConditionType.UsingFanning;
    }
    public class NotUsedFanningThisTurnCondition : NotActedThisTurnCondition<FanningAction>
    {
        public NotUsedFanningThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedFanningThisTurn;
    }
    #endregion
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
