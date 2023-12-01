using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(fileName = "ActiveDatabase", menuName = "ScriptableObjects/ActiveDatabase", order = 0)]
public class ActiveDatabase : ScriptableObject
{
    #region STATIC
    ActionType GetActionRange(string actionName)
    {
        return actionName switch
        {
            "Fanning" => ActionType.Fanning,
            "Dynamite" => ActionType.Dynamite,
            "Hemostasis" => ActionType.Hemostasis,
            _ => ActionType.None
        };
    }
    #endregion
    
    public List<ActiveInfo> infos;
    
    [ContextMenu("Read Csv")]
    public void ReadCsv()
    {
        var infoList = FileRead.Read("ActiveSkillTable");

        if (infos is null) infos = new List<ActiveInfo>();
        else infos.Clear();
        
        foreach(var info in infoList)
        {
            var curInfo = new ActiveInfo
            {
                index = int.Parse(info[(int)Column.Index]),
                action = GetActionRange(info[(int)Column.Effect]),
                amounts = FileRead.ConvertStringToArray<float>(info[(int)Column.EffectAmount]),
                commentary = info[(int)Column.Commentary],
                damage = int.Parse(info[(int)Column.Damage]),
                cost = int.Parse(info[(int)Column.Cost]),
                range = int.Parse(info[(int)Column.Range]),
                radius = int.Parse(info[(int)Column.Radius]),
                ammoCost = 0//int.Parse(info[(int)Column.AmmoCost]),
            };
            
            infos.Add(curInfo);
        }
    }

    public void GetAction(Unit unit, int index)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].index == index)
            {
                var info = infos[i];
                
                Debug.Log("Add Component");
                string componentName = info.action + "Action";
                IUnitAction action = 
                    unit.gameObject.AddComponent(Type.GetType(componentName)) as IUnitAction;

                action?.SetData(info);
                break;
            }
        }
        
    }

    public ActiveInfo GetActiveInfo(int index)
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
        return new ActiveInfo();
    }
}

[Serializable]
public struct ActiveInfo
{
    public int index;
    public ActionType action;
    public float[] amounts;

    public string commentary;
    public int damage;
    public int cost;
    public int ammoCost; 
    public int range;
    public int radius;
}

internal enum Column
{
    Index,
    Effect,
    EffectAmount,
    Commentary, 
    Damage,
    Cost,
    Range,
    Radius,
    AmmoCost,
}

