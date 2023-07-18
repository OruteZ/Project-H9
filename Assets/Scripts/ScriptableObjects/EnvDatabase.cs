using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvDatabase", menuName = "ScriptableObjects/EnvDatabase", order = 1)]
public class EnvDatabase : ScriptableObject
{
    [SerializeField] private List<EnvironmentData> dataList;
    
    public GameObject GetEnvPrefab(int index)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].index == index) return dataList[i].prefab;
        }
        
        Debug.LogError("There is no Env object that has index " + index);
        return null;
    }
}

[System.Serializable]
public struct EnvironmentData
{
    public int index;
    public GameObject prefab;
}
