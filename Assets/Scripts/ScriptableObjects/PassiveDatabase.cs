using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(fileName = "PassiveDatabase", menuName = "ScriptableObjects/PassiveDatabase", order = 0)]
public class PassiveDatabase : ScriptableObject
{
    [SerializeField] 
    private List<PassiveInfo> infos;

    [ContextMenu("Read Csv")]
    public void ReadCsv()
    {
        var infoList = FileRead.Read("Passive");

        if (infos is null) infos = new List<PassiveInfo>();
        else infos.Clear();
        
        foreach(var info in infoList)
        {
            var curInfo = new PassiveInfo
            {
                index = int.Parse(info[(int)Column.Index]),

                condition = (ConditionType)int.Parse(info[(int)Column.Condition]),
                conditionAmount = int.Parse(info[(int)Column.ConditionAmount]),

                effect = (PassiveEffectType)int.Parse(info[(int)Column.Effect]),
                effectStat = (UnitStatType)int.Parse(info[(int)Column.EffectStat]),
                effectAmount = int.Parse(info[(int)Column.EffectAmount]),
            };
            
            infos.Add(curInfo);
        }
    }

    public Passive GetPassive(int index, Unit owner)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            if (info.index == index)
            {
                var trigger = Passive.CloneTrigger(info.condition, info.conditionAmount);
                var effect = Passive.CloneEffect(info.effect, info.effectStat, info.effectAmount);
                return new Passive(owner, trigger, effect);
            }
        }

        return null;
    }
}

[Serializable]
public struct PassiveInfo
{
    public int index;
    
    public ConditionType condition;
    public float conditionAmount;
    
    public PassiveEffectType effect;
    public UnitStatType effectStat;
    public float effectAmount;
}

internal enum Column
{
    Index,
    Condition,
    ConditionAmount,
    Effect,
    EffectStat,
    EffectAmount,
}