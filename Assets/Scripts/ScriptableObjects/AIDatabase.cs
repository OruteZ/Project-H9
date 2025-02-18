
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BTDatabase", menuName = "ScriptableObjects/BTDatabase", order = 1)]
public class AIDatabase : ScriptableObject
{
    [SerializeField] private List<AIModel> btList;

    public AIModel GetTree(int index)
    {
        return btList[index - 1];
    }
}