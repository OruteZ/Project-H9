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
        List<List<string>> infoList = FileRead.Read("PassiveSkillTable", out Dictionary<string, int> columnInfo);

        if (infos is null) infos = new List<PassiveInfo>();
        else infos.Clear();
        
        foreach(List<string> info in infoList)
        {
            string[] conditionStrings = info[(int)PassiveColumn.Condition].Trim().Trim('\"').Split(',');
            string[] conditionAmountStrings = info[(int)PassiveColumn.ConditionAmount].Trim().Trim('\"').Split(',');
            string[] effectStrings = info[(int)PassiveColumn.Effect].Trim().Trim('\"').Split(',');
            string[] effectStatStrings = info[(int)PassiveColumn.EffectStat].Trim().Trim('\"').Split(',');
            string[] effectAmountStrings = info[(int)PassiveColumn.EffectAmount].Trim().Trim('\"').Split(',');
            if (conditionStrings.Length != conditionAmountStrings.Length ||
                effectStrings.Length != effectStatStrings.Length ||
                effectStrings.Length != effectAmountStrings.Length) 
            {
                Debug.LogError("패시브 스킬 테이블의 복합 조건, 복합 효과 개수가 잘못되어있습니다.");
                return;
            }


            ConditionType[] tmpCondition = new ConditionType[conditionStrings.Length];
            float[] tmpConditionAmount = new float[conditionStrings.Length];
            for (int i = 0; i < conditionStrings.Length; i++)
            {
                tmpCondition[i] = (ConditionType)int.Parse(conditionStrings[i]);
                tmpConditionAmount[i] = float.Parse(conditionAmountStrings[i]);
            }

            PassiveEffectType[] tmpEffect = new PassiveEffectType[effectStrings.Length];
            StatType[] tmpEffectStat = new StatType[effectStrings.Length];
            int[] tmpEffectAmount = new int[effectStrings.Length];
            for (int i = 0; i < effectStrings.Length; i++)
            {
                tmpEffect[i] = (PassiveEffectType)int.Parse(effectStrings[i]);
                tmpEffectStat[i] = (StatType)int.Parse(effectStatStrings[i]);
                tmpEffectAmount[i] = int.Parse(effectAmountStrings[i]);
            }


            var curInfo = new PassiveInfo
            {
                index = int.Parse(info[(int)PassiveColumn.Index]),
                condition = tmpCondition,
                conditionAmount = tmpConditionAmount,

                effect = tmpEffect,
                
                effectStat = tmpEffectStat,
                effectAmount = tmpEffectAmount,
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
        Debug.LogError("Can't Find Passive: index = " + index);

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
    
    public ConditionType[] condition;
    public float[] conditionAmount;
    
    public PassiveEffectType[] effect;
    public StatType[] effectStat;
    public int[] effectAmount;
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