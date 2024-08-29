using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedFanningThisTurnCondition : BaseCondition
    {
        public UsedFanningThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedFanningThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is FanningAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class UsingFanningCondition : BaseCondition
    {
        public UsingFanningCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingFanning;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is FanningAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is FanningAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotUsedFanningThisTurnCondition : BaseCondition
    {
        public NotUsedFanningThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedFanningThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is FanningAction) passive.NotFullfillCondition(this);
        }
        private void CheckTurnOwner()
        {
            if (FieldSystem.turnSystem.turnOwner == unit)
            {
                passive.FullfillCondition(this);
            }
        }
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
