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
        var infoList = FileRead.Read("PassiveSkillTable", out var columnInfo);

        if (infos is null) infos = new List<PassiveInfo>();
        else infos.Clear();
        
        foreach(var info in infoList)
        {
            var curInfo = new PassiveInfo
            {
                index = int.Parse(info[(int)PassiveColumn.Index]),

                condition = (ConditionType)int.Parse(info[(int)PassiveColumn.Condition]),
                conditionAmount = int.Parse(info[(int)PassiveColumn.ConditionAmount]),

                effect = (PassiveEffectType)int.Parse(info[(int)PassiveColumn.Effect]),
                
                //maxHp는 사용안하므로 CSV상 null과 대응, maxHp를 건들려면 이부분 수정해야 함
                effectStat = (StatType)int.Parse(info[(int)PassiveColumn.EffectStat]),
                effectAmount = int.Parse(info[(int)PassiveColumn.EffectAmount]),
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
                return new Passive(owner, index, trigger, effect);
            }
        }

        return null;
    }

    public PassiveInfo GetPassiveInfo(int index)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            if (info.index == index)
            {
                return info;
            }
        }
        Debug.LogError(index);
        return new PassiveInfo();
    }
}

[Serializable]
public struct PassiveInfo
{
    public int index;
    
    public ConditionType condition;
    public float conditionAmount;
    
    public PassiveEffectType effect;
    public StatType effectStat;
    public int effectAmount;
}

internal enum PassiveColumn
{
    Index,
    Condition,
    ConditionAmount,
    Effect,
    EffectStat,
    EffectAmount,
}