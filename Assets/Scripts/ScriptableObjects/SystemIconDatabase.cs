using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemIconDatabase", menuName = "ScriptableObjects/SystemIconDatabase", order = 1)]
public class SystemIconDatabase : ScriptableObject
{
    [SerializeField]
    public List<IconInfo> iconList;

    public Sprite GetIconInfo(int index)
    {
        for (int i = 0; i < iconList.Count; i++)
        {
            var info = iconList[i];
            if (info.index == index)
            {
                return info.icon;
            }
        }
        Debug.LogError(index);
        return iconList[0].icon;
    }
    public Sprite GetIconInfo(string name)
    {
        for (int i = 0; i < iconList.Count; i++)
        {
            var info = iconList[i];
            if (info.name == name)
            {
                return info.icon;
            }
        }
        Debug.LogError($"{name} 아이콘을 찾을 수 없습니다.");
        return iconList[0].icon;
    }
}

[Serializable]
public struct IconInfo
{
    public int index;
    public string name;
    public Sprite icon;
}
