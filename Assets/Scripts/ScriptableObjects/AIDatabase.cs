
using System.Collections.Generic;
using KieranCoppins.DecisionTrees;
using UnityEngine;

[CreateAssetMenu(fileName = "BTDatabase", menuName = "ScriptableObjects/BTDatabase", order = 1)]
public class AIDatabase : ScriptableObject
{
    [SerializeField] private List<DecisionTree> btList;

    public DecisionTree GetTree(int index)
    {
        return btList[index - 1];
    }
}