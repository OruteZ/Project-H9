using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "PassiveDatabase", menuName = "ScriptableObjects/PassiveDatabase", order = 0)]
public class PassiveDatabase : ScriptableObject
{
    [SerializeField] 
    private List<PassiveInfo> infos;

    public Passive GetPassive(int index, Unit owner)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            if (info.index == index)
            {
                var trigger = Passive.CloneTrigger(info.trigger, info.triggerAmount);
                var effect = Passive.CloneEffect(info.passiveEffect, info.effectAmount);
                return new Passive(owner, trigger, effect);
            }
        }

        return null;
    }

    public void AddPassiveInfo(PassiveInfo info)
    {
        infos.Add(info);
    }
}

[Serializable]
public struct PassiveInfo
{
    public int index;
    
    public TriggerType trigger;
    public float triggerAmount;
    
    public PassiveEffectType passiveEffect;
    public float effectAmount;
}