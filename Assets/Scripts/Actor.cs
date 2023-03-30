using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public abstract class Actor : MonoBehaviour
{
    public HexTransform hexTransform;
    ///<value>한칸을 이동하는데 걸리는 시간(초)입니다.</value>
    public float oneTileMoveTime;
    public WorldMap world;

    [Header("For Debug")]
    [SerializeField] private ActorState state;

    [Header("Control Inspector")] 
    [SerializeField] private int speed;

    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
    }

    private void Start()
    {
        hexTransform.position = Vector3Int.zero;
        state = ActorState.EndTurn;
    }

    private void Update()
    {
        switch (state)
        {
            case ActorState.StartTurn:
                break;
            case ActorState.SelectingAct:
                SelectingAct();
                break;
            case ActorState.Acting:
                break;
            case ActorState.EndTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract void SelectingAct();
    protected abstract void Act();

    public void StartTurn()
    {
        state = ActorState.StartTurn;
    }

    private Tile GetMouseOverTile()
    {
        RaycastHit hit;
        if (Camera.main == null) return null;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit)) return null;

        return hit.transform.GetComponent<Tile>();
    }
}

public enum ActorState
{
    StartTurn,
    SelectingAct,
    Acting,
    FinishAct,
    EndTurn
}
