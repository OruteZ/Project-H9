using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
// ReSharper disable InconsistentNaming

public static class Hex
{
    //육각형 타일 하나의 반지름 크기를 의미합니다.
    public const float Radius = 1f;
    
    //루트 3 상수입니다.
    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    /// <summary>
    /// Hex좌표를 인게임 World 직교좌표계로 변환합니다.
    /// </summary>
    /// <param name="position">Cube 표현방식의 Hex좌표입니다.</param>
    /// <returns>World의 좌표계</returns>
    public static Vector3 Hex2World(Vector3 position)
    {
        var x = Radius * (Sqrt3 * position.x + Sqrt3 * 0.5f * position.y);
        var y = -position.y * 1.5f;

        return new Vector3(x, 0, y);
    }
    
    /// <summary>
    /// 두 타일간의 거리를 반환합니다.
    /// </summary>
    /// <param name="pos1">Hex좌표 1</param>
    /// <param name="pos2">Hex좌표 2</param>
    /// <returns>두 타일 사이 거리</returns>
    public static int Distance(Vector3Int pos1, Vector3Int pos2)
    {
        var vec = pos1 - pos2;
        return (Mathf.Abs(vec.x) + Mathf.Abs(vec.y) + Mathf.Abs(vec.z)) / 2;
    }
    
    /// <summary>
    /// start좌표에서 end좌표로 향하는 직선 경로를 담은 List를 반환합니다. 시작점과 도착점의 좌표도 List에 포함됩니다.
    /// </summary>
    /// <param name="start">시작점의 Hex좌표</param>
    /// <param name="end">도착점의 Hex좌표</param>
    /// <returns>Hex 좌표를 담은 List</returns>
    public static List<Vector3Int> LineDraw(Vector3Int start, Vector3Int end)
    {
        var dist = Distance(start, end);

        var start_nudge = new Vector3((float)(start.x + 1e-6), (float)(start.y + 1e-6), (float)(start.z - 2e-6));
        var end_nudge = new Vector3((float)(end.x + 1e-6), (float)(end.y + 1e-6), (float)(end.z - 2e-6));
        
        var results = new List<Vector3Int>();
        for (var i = 0; i <= dist; i++)
        {
            results.Add(Round(Vector3.Lerp(start_nudge, end_nudge, 1.0f / dist * i)));
            //results.Add(Round(Vector3.Lerp(start, end, 1.0f / dist * i)));
        }

        return results;
    }
    
    public static List<Vector3Int> LineDraw_(Vector3Int start, Vector3Int end)
    {
        var dist = Distance(start, end);

        var start_nudge = new Vector3((float)(start.x - 1e-6), (float)(start.y - 1e-6), (float)(start.z + 2e-6));
        var end_nudge = new Vector3((float)(end.x - 1e-6), (float)(end.y - 1e-6), (float)(end.z + 2e-6));
        
        var results = new List<Vector3Int>();
        for (var i = 0; i <= dist; i++)
        {
            results.Add(Round(Vector3.Lerp(start_nudge, end_nudge, 1.0f / dist * i)));
            //results.Add(Round(Vector3.Lerp(start, end, 1.0f / dist * i)));
        }

        return results;
    }
    
    /// <summary>
    /// 실수형으로 이루어진 Hex좌표를 가장 가까운 정수형 Hex 좌표로 반올림합니다.
    /// </summary>
    /// <param name="vec">실수형 Hex좌표</param>
    /// <returns>정수형 Hex좌표</returns>
    private static Vector3Int Round(Vector3 vec)
    {
        var x = Mathf.RoundToInt(vec.x);
        var y = Mathf.RoundToInt(vec.y);
        var z = Mathf.RoundToInt(vec.z);

        var xDiff = Mathf.Abs(x - vec.x);
        var yDiff = Mathf.Abs(y - vec.y);
        var zDiff = Mathf.Abs(z - vec.z);

        if (xDiff > yDiff && xDiff > zDiff)
        {
            x = -y - z;
        }
        else if (yDiff > zDiff)
        {
            y = -x - z;
        }
        else
        {
            z = -x - y;
        }

        return new Vector3Int(x, y, z);
    }
    
    /// <summary>
    /// 중심점에서 직선거리가 일정 거리 이하인 모든 Hex좌표를 List로 반환합니다.
    /// </summary>
    /// <param name="range">직선거리</param>
    /// <param name="center">중심점의 Hex좌표</param>
    /// <returns>범위 내부 Hex좌표들의 List</returns>
    public static List<Vector3Int> GetGridsWithRange(int range, Vector3Int center)
    {
        var results = new List<Vector3Int>();
        for (var x = -range; x <= range; x++)
        {
            for (var y = Mathf.Max(-range, -x - range); y <= Mathf.Min(range, -x + range); y++)
            {
                var z = -x - y;
                results.Add(center + new Vector3Int(x, y, z));
            }
        }
        return results;
    }

    public static Vector3Int upLeft => new Vector3Int(-1, 1, 0);
    public static Vector3Int upRight => new Vector3Int(0, 1, -1);
    public static Vector3Int left => new Vector3Int(-1, 0, 1);
    public static Vector3Int right => new Vector3Int(1, 0, -1);
    public static Vector3Int downRight => new Vector3Int(1, -1, 0);
    public static Vector3Int downLeft => new Vector3Int(0, -1, 1);
    public static Vector3Int zero => new Vector3Int(0, 0, 0);

    public static IEnumerable<Vector3Int> directions => new[]
    {
        upLeft,
        upRight,
        left,
        right,
        downLeft,
        downRight
    };
}
