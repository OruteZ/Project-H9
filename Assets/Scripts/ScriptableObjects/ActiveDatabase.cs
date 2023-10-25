﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(fileName = "ActiveDatabase", menuName = "ScriptableObjects/ActiveDatabase", order = 0)]
public class ActiveDatabase : ScriptableObject
{
    public List<ActiveInfo> infos;
    
    [ContextMenu("Read Csv")]
    public void ReadCsv()
    {
        var infoList = FileRead.Read("ActiveTable");

        if (infos is null) infos = new List<ActiveInfo>();
        else infos.Clear();
        
        foreach(var info in infoList)
        {
            var curInfo = new ActiveInfo
            {
                index = int.Parse(info[0]),
                action = ActionType.Fanning,
                amounts = FileRead.ConvertStringToArray<float>(info[2])
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
                switch (info.action)
                {
                    case ActionType.Fanning:
                        unit.AddComponent<FanningAction>().SetAmount(info.amounts);
                        break;
                    case ActionType.Dynamite:
                        unit.AddComponent<DynamiteAction>().SetAmount(info.amounts);
                        break;
                }

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
}