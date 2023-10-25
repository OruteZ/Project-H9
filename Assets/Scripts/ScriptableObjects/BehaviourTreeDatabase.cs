
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BTDatabase", menuName = "ScriptableObjects/BTDatabase", order = 1)]
public class BehaviourTreeDatabase : ScriptableObject
{
    [SerializeField] private List<BehaviourTree> btList;

    public BehaviourTree GetTree(int index)
    {
        return btList[index - 1];
    }
}