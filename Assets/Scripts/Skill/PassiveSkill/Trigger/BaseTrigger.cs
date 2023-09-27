using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassiveSkill
{
    public abstract class BaseTrigger : ITrigger
    {
        protected float amount;
        
        protected Passive passive;
        public Unit unit => passive.unit;
        
        public BaseTrigger(float amt)
        {
            SetAmount(amt);
        }
    
        public void Setup(Passive passive)
        {
            this.passive = passive;
            TriggerSetup();
        }
        public void SetAmount(float amt)
        {
            amount = amt;
        }

        public abstract TriggerType GetTriggerType();
        protected abstract void TriggerSetup();
    }
}

