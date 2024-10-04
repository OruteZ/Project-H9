using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
// ReSharper disable InconsistentNaming

public static class Hex
{
    public static Vector3Int ColToHex(Vector2Int cooPos)
    {
        var x = cooPos.x - (cooPos.x - (cooPos.x & 1)) / 2;
        var y = cooPos.y;

        return new Vector3Int(x, y, - x - y);
    }

    public static Vector2Int HexToCol(Vector3Int hex)
    {
        var col = hex.x + (hex.y - (hex.y & 1)) / 2;
        var row = hex.y;

        return new Vector2Int(col, row); 
    }
    
    //육각형 타일 하나의 반지름 크기를 의미합니다.
    public const float Radius = 1f;
    
    public const float InnerRadius = Radius * 0.866025404f;
    
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
    
    public static Vector2 Hex2Orth(Vector3 position)
    {
        float x = Radius * (Sqrt3 * position.x + Sqrt3 * 0.5f * position.y);
        float y = -position.y * 1.5f;

        return new Vector2(x, y);
    }

    public static Vector3 World2Hex(Vector3 position)
    {
        var y =  -position.z * (2f/3f);
        var x = ((Sqrt3 * position.x) / (Radius * 3)) - (0.5f * y);

        return new Vector3(x, y, -(x + y));
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
    public static List<Vector3Int> DrawLine1(Vector3Int start, Vector3Int end)
    {
        var dist = Distance(start, end);

        var start_nudge = new Vector3((float)(start.x + 1e-6), (float)(start.y - 1e-6), (float)(start.z));
        var end_nudge = new Vector3((float)(end.x + 1e-6), (float)(end.y - 1e-6), (float)(end.z));
        
        var results = new List<Vector3Int>();
        for (var i = 0; i <= dist; i++)
        {
            results.Add(Round(Vector3.Lerp(start_nudge, end_nudge, 1.0f / dist * i)));
            //results.Add(Round(Vector3.Lerp(start, end, 1.0f / dist * i)));
        }

        return results;
    }
    
    public static List<Vector3Int> DrawLine2(Vector3Int start, Vector3Int end)
    {
        var dist = Distance(start, end);

        var start_nudge = new Vector3((float)(start.x - 1e-6), (float)(start.y + 1e-6), (float)(start.z));
        var end_nudge = new Vector3((float)(end.x - 1e-6), (float)(end.y + 1e-6), (float)(end.z));
        
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
    public static Vector3Int Round(Vector3 vec)
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
    public static List<Vector3Int> GetCircleGridList(int range, Vector3Int center)
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
    
    /// <summary>
    /// 중심점에서 직선거리가 일정 거리인 모든 Hex좌표를 List로 반환합니다.
    /// </summary>
    /// <param name="range">직선거리</param>
    /// <param name="center">중심점의 Hex좌표</param>
    /// <returns>범위 내부 Hex좌표들의 List</returns>
    public static List<Vector3Int> GetCircleLineGridList(int range, Vector3Int center)
    {
        var results = new List<Vector3Int>();
        for (var x = -range; x <= range; x++)
        {
            for (var y = Mathf.Max(-range, -x - range); y <= Mathf.Min(range, -x + range); y++)
            {
                var z = -x - y;
                var hex = center + new Vector3Int(x, y, z);
                if (Distance(center, hex) == range)
                {
                    results.Add(hex);
                }
            }
        }
        return results;
    }


    /// <summary>
    /// 가로 세로가 주어진 직사각형 모양의 Grid List를 반환합니다.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static List<Vector3Int> GetSquareGridList(int width, int height, Vector3Int start)
    {
        var results = new List<Vector3Int>();
        
        for (int h = 0; h < height; h++)
        {
            int w_start = -(h / 2);
            for (int w = w_start; w < w_start + width; w++)
            {
                results.Add(new Vector3Int(w, h, -(w + h)) + start);
            }
        }

        return results;
    }
    
    public static float GetRotateAngle(Vector3Int from, Vector3Int to)
    {
        Vector2 fromPos = Hex2Orth(from);
        Vector2 toPos = Hex2Orth(to);
        
        float angle = Vector2.SignedAngle(Vector2.up, toPos - fromPos);
        return angle;
    }

    public static Vector3Int GetDirectionHex(Direction dir)
    {
        switch (dir)
        {
            case Direction.UpLeft:
                return upLeft;
            case Direction.UpRight:
                return upRight;
            case Direction.Left:
                return left;
            case Direction.Right:
                return right;
            case Direction.DownLeft:
                return downLeft;
            case Direction.DownRight:
                return downRight;
            default:
                return none;
        }
    }

    // public static Vector3Int upLeft => new Vector3Int(-1, 1, 0);
    // public static Vector3Int upRight => new Vector3Int(0, 1, -1);
    // public static Vector3Int left => new Vector3Int(-1, 0, 1);
    // public static Vector3Int right => new Vector3Int(1, 0, -1);
    // public static Vector3Int downRight => new Vector3Int(1, -1, 0);
    // public static Vector3Int downLeft => new Vector3Int(0, -1, 1);
    public static Vector3Int zero => new Vector3Int(0, 0, 0);
    public static Vector3Int none => new Vector3Int(-1, -1, -1);
    public static Vector3Int downRight => new Vector3Int(0, 1, -1);
    public static Vector3Int downLeft => new Vector3Int(-1, 1, 0);
    public static Vector3Int right => new Vector3Int(1, 0, -1);
    public static Vector3Int left => new Vector3Int(-1, 0, 1);
    public static Vector3Int upRight => new Vector3Int(0, -1, 1);
    public static Vector3Int upLeft => new Vector3Int(-1, 0, 1);
    

    public static IEnumerable<Vector3Int> directions => new[]
    {
        upLeft,
        upRight,
        left,
        right,
        downLeft,
        downRight
    };

    public enum Direction
    {
        Right,
        DownRight,
        DownLeft ,
        Left,
        UpLeft,
        UpRight,
    }
}
