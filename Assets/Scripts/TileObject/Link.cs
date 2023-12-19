using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Link : TileObject
{
    private static LinkDatabase _linkDatabase;
    
    private int _linkIndex;

    public int linkIndex
    {
        get => _linkIndex;
        set
        {
            _linkIndex = value;
            ReloadModel();
        }
    }
    public int combatMapIndex;

    private bool _vision;
    
    private bool IsEncounterEnable()
    {
        int curTurn = FieldSystem.turnSystem.turnNumber;
        bool hasFinished = EncounterManager.instance.TryGetTurn(hexPosition, out int lastTurn);

        if (hasFinished is false) return true;
        return lastTurn + 5 <= curTurn;
    }
    public override void OnCollision(Unit other)
    {
        if (IsEncounterEnable() is false) return;

        other.GetSelectedAction().ForceFinish();
        
        Debug.Log("On Collision Calls");
        EncounterManager.instance.AddValue(hexPosition, FieldSystem.turnSystem.turnNumber);
        GameManager.instance.StartCombat(combatMapIndex, linkIndex: linkIndex);
    }

    public override void SetVisible(bool value)
    {
        //if editor mode, value always true
        if (GameManager.instance.CompareState(GameState.Editor)) value = true;
        
        meshRenderer.enabled = value && IsEncounterEnable();
        _vision = value;
    }

    public override string[] GetArgs()
    {
        return new [] { linkIndex.ToString() };
    }

    public override void SetArgs(string[] args)
    {
        linkIndex = int.Parse(args[0]);
    }
    
    public override void SetUp()
    {
        base.SetUp();
        
        //Link는 World Object라서 한번 밝혀지면 상관이 없지만 
        //턴이 바뀜에 따라서 안보이는게 보일 수 있으니 확인 해줘야 함
        FieldSystem.turnSystem.onTurnChanged.AddListener(() => SetVisible(_vision));
    }

    private void ReloadModel()
    {
        //load link database
        _linkDatabase ??= Resources.Load<LinkDatabase>("Database/LinkDatabase");
        //check null
        if (_linkDatabase == null)
        {
            Debug.LogError("LinkDatabase is null");
            return;
        }

        //remove all child
        var children = new Stack<GameObject>();
        foreach (Transform child in transform) children.Push(child.gameObject);
        while (children.Count > 0) DestroyImmediate(children.Pop());

        //get model from link database and instantiate
        var model = Instantiate(_linkDatabase.GetData(linkIndex).model, transform);
        
        //set model local position zero
        model.transform.localPosition = Vector3.zero;
        
        //set model local rotation zero
        model.transform.localRotation = Quaternion.identity;
        
        //set model's animator to "Const Standing Idle" by load Resource
        var animator = model.GetComponent<Animator>();
        animator.runtimeAnimatorController = 
            Resources.Load<RuntimeAnimatorController>("Animator/Const Standing Idle");
    }
}