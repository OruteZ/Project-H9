using System.Collections.Generic;
using UnityEngine;

public class Vector3IntCompare : IComparer<Vector3Int>
{
    public int Compare(Vector3Int x, Vector3Int y)
    {
        if (x.x < y.x)
            return -1;
        if (x.x > y.x)
            return 1;
        if (x.y < y.y)
            return -1;
        if (x.y > y.y)
            return 1;
        if (x.z < y.z)
            return -1;
        if (x.z > y.z)
            return 1;
        return 0;
    }
}