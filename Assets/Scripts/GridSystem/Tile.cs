using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    private HexTransform _hexTransform;

    public Vector3Int hexPosition
    {
        get
        {
            _hexTransform ??= GetComponent<HexTransform>();
            return _hexTransform.position;
        }
        set
        {
            _hexTransform ??= GetComponent<HexTransform>();
            _hexTransform.position = value;
        } 
    }

    [Header("타일 속성")] 
    public bool walkable;
    public bool visible;
    public bool rayThroughable;

    [Header("플레이어 시야")] 
    [SerializeField] 
    private bool _inSight;

    public List<TileObject> objects;
    protected void Awake()
    {
        _hexTransform = GetComponent<HexTransform>();
    }

    public void AddObject(TileObject u)
    {
        objects.Add(u);
    }

    public void RemoveObject(TileObject u)
    {
        objects.Remove(u);
    }

    public bool inSight
    {
        get => _inSight;
        set
        {
            _inSight = value;
            for (var index = 0; index < objects.Count; index++)
            {
                
                var obj = objects[index];
                if (obj is FogOfWar fow)
                {
                    fow.SetVisible(value);
                    if (value) index--;
                }
                
                
                //Combat일때만 value 따라감
                else if (GameManager.instance.CompareState(GameState.Combat))
                {
                    obj.SetVisible(value);
                }
                //월드Scene일 경우 무조건 true
                else
                {
                    obj.SetVisible(true);
                }
            }

            var unit = FieldSystem.unitSystem.GetUnit(hexPosition);
            if (unit is not null) unit.isVisible = value;
        }
    }

    
}