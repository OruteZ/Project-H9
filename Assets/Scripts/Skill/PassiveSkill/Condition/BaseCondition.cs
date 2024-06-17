using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassiveSkill
{
    public abstract class BaseCondition : ICondition
    {
        protected float amount;

        protected Passive passive;
        public Unit unit => passive.unit;


        public BaseCondition(float amt)
        {
            SetAmount(amt);
        }

        public void Setup(Passive passive)
        {
            this.passive = passive;
            ConditionSetup();
        }

        public void SetAmount(float amt)
        {
            amount = amt;
        }

        public abstract ConditionType GetConditionType();
        protected abstract void ConditionSetup();
        
        public virtual void OnDelete()
        {
            
        }
    }
}

