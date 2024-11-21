using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class Link : TileObject
{
    private static LinkDatabase _linkDatabase;
    
    [SerializeField]
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
    public int combatMapIndex = 0;
    public bool isRepeatable = false;

    private bool _vision;

    private bool IsEncounterEnable()
    {
        return true;
        
        // int curTurn = FieldSystem.turnSystem.turnNumber;
        // bool hasFinished = EncounterManager.instance.TryGetTurn(hexPosition, out int lastTurn);
        //
        // // 이미 죽은 적이 없으면 : return true
        // if (hasFinished is false) return true;
        //
        // // 마지막을 전투한 시점 추가
        // EncounterManager.instance.AddValue(hexPosition, 
        //     FieldSystem.turnSystem.turnNumber +
        //     (int.MaxValue / 2));
        //
        // // todo : 나중에 싹 뜯어고쳐야 함, 위치를 기반으로 링크의 부활 여부를 관리하는것도 문제, 일회용 링크여도 삭제되지 않는것도 문제
        // if (isRepeatable) return lastTurn + 5 <= curTurn;
        // else return false;
    }
    public override void OnCollision(Unit other)
    {
        if (IsEncounterEnable() is false) return;

        other.GetSelectedAction().ForceFinish();
        
        Debug.Log("On Collision Calls");
        EncounterManager.instance.AddValue(hexPosition, FieldSystem.turnSystem.turnNumber);
        GameManager.instance.StartCombat(tile.combatStageIndex, linkIndex: linkIndex);
        
        Remove();
    }

    public void Remove()
    {
        if (isRepeatable is false)
        {
            // Destroy(gameObject);
            //call gameManager's runtime world data
            GameManager.instance.runtimeWorldData.RemoveLink(hexPosition, _linkIndex);
        }
    }

    public override void SetVisible(bool value)
    {
        //if editor mode, value always true
        if (GameManager.instance.CompareState(GameState.EDITOR)) value = true;
        
        meshRenderer.enabled = value && IsEncounterEnable();
        _vision = value;
    }

    public override string[] GetArgs()
    {
        return new [] { linkIndex.ToString(), transform.rotation.y.ToString(CultureInfo.InvariantCulture) };
    }

    public override void SetArgs(string[] args)
    {
        if(args.Length != 2) throw new ArgumentException("Link SetArgs Error");

        linkIndex = int.Parse(args[0]);
        transform.rotation = Quaternion.Euler(0, float.Parse(args[1]), 0);
    }
    
    public override void SetUp()
    {
        base.SetUp();
        
        //Link는 World Object라서 한번 밝혀지면 상관이 없지만 
        //턴이 바뀜에 따라서 안보이는게 보일 수 있으니 확인 해줘야 함
        FieldSystem.turnSystem.onTurnChanged.AddListener(() => SetVisible(
            GameManager.instance.IsPioneeredWorldTile(hexPosition)
            ));
    }

    private void ReloadModel()
    {
        //load link database
        _linkDatabase ??= Resources.Load<LinkDatabase>($"Database/LinkDatabase");
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
        
        //reload renderer
        InitRenderer();
    }

    public int GetCombatStageIndex()
    {
        if(combatMapIndex != 0) return combatMapIndex;
        return tile.combatStageIndex;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (GameManager.instance.CompareState(GameState.EDITOR)) return;
        if (_linkIndex == 0)
        {
            Debug.LogError("link index가 0입니다! 플레이를 끄고 빨리 고치세요 휴먼!" +
                           "현재 위치 : " + hexPosition);
            
        }
        // if (combatMapIndex == 0)
        // {
        //     Debug.LogError("combat map index가 0입니다! 플레이를 끄고 빨리 고치세요 휴먼!");
        // }
    }
#endif
}