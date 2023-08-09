using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EncounterManager : Generic.Singleton<EncounterManager>
{
    private readonly Dictionary<Vector3Int, int> _combatFinishData = new Dictionary<Vector3Int, int>();

    public bool TryGetTurn(Vector3Int position, out int turn)
    {
        return _combatFinishData.TryGetValue(position, out turn);
    }

    public void AddValue(Vector3Int position, int turn)
    {
        bool exist =
            _combatFinishData.ContainsKey(position);

        if (exist is false)
        {
            _combatFinishData.TryAdd(position, turn);
        }
        else
        {
            _combatFinishData[position] = turn;
        }
    }
}
