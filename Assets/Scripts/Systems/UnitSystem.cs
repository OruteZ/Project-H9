using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using KieranCoppins.DecisionTrees;
using UnityEngine.Serialization;

public class UnitSystem : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private WeaponDatabase weaponDB;
    [SerializeField] private EnemyDatabase enemyDB;
    [SerializeField] private PassiveDatabase passiveDB;
    [SerializeField] private ActiveDatabase activeDB;
    [SerializeField] private LinkDatabase linkDB;

    private const string AI_PATH = "AI/";
    //---
    
    [Space(10)]
    public List<Unit> units;
    public UnityEvent<Unit> onAnyUnitMoved;
    public UnityEvent<Unit> onAnyUnitDead;
    public UnityEvent<Unit> onUnitCreated;
    
    [SerializeField] private Transform unitParent;
    [SerializeField] private GameObject playerPrefab;
    
    [FormerlySerializedAs("enemyPrefab")] [SerializeField] private GameObject[] enemyPrefabs;
    
    private int _totalExp;
    public CombatRewardHandler rewardHandler;

    private CustomOutline.Mode[] outlineMode = { CustomOutline.Mode.NULL, CustomOutline.Mode.OutlineAll, CustomOutline.Mode.SilhouetteOnly };
    private int modeIndex = 2;

    private void Start()
    {
        SetPlayerOutline();
        FieldSystem.onStageStart.AddListener(SetPlayerOutline);
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
    }
    private void Update()
    {
        if (Input.GetKeyDown(HotKey.changeOutlineModeKey)) 
        {
            modeIndex++;
            SetPlayerOutline();
        }
    }
    private void SetPlayerOutline()
    {
        Player player = GetPlayer();
        if (player is null) return;
        player.TryGetComponent(out CustomOutline outline);
        if (outline is null)
        {
            outline = player.AddComponent<CustomOutline>();
            outline.OutlineColor = Color.red;
        }

        if (modeIndex >= outlineMode.Length) modeIndex = 0;
        outline.OutlineMode = outlineMode[modeIndex];
    }

    /// <summary>
    /// 유닛을 생성하고, 유닛의 데이터를 초기화합니다.
    /// </summary>
    public void SetUpUnits()
    {
        _totalExp = 0;
        rewardHandler = new CombatRewardHandler(GameManager.instance.itemDatabase);

        //get all link data and instantiate enemy
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            var linkDataIdx = GameManager.instance.GetLinkIndex();
            var linkData = linkDB.GetData(linkDataIdx);
            var enemyCount = linkData.combatEnemy.Length;

            var mapSpawnData = GameManager.instance.GetStageData();
            if (!mapSpawnData.TryGetEnemyPoints(linkData, out var enemySpawnPoints))
            {
                Debug.LogError("해당 링크의 Enemy Spawn Point is null");
                return;
            }

            if (!mapSpawnData.TryGetPlayerPoint(out Vector3Int playerSpawnPoint))
            {
                Debug.LogError("해당 링크의 Player Spawn Point is null");
                return;
            }
            
            //create player
            var player = Instantiate(playerPrefab, unitParent).GetComponent<Player>();
            player.hexPosition = playerSpawnPoint;
            units.Add(player);
            
            //random spawn enemy by spawn points
            var enemySpawnPointList = enemySpawnPoints.ToList();
            
            //if count is not same, throw error
            if (enemyCount != enemySpawnPointList.Count)
            {
                Debug.LogError("Enemy Count is not same with spawn point count : " +
                               enemyCount + " / " + enemySpawnPointList.Count
                               + " linkDataIdx : " + linkDataIdx+
                               " linkData.combatEnemy.Length : " + linkData.combatEnemy.Length);
                throw new Exception();
            }
            
            for (int i = 0; i < enemyCount; i++)
            {
                int enemyIdx = linkData.combatEnemy[i];
                
                // get enemy data
                EnemyData info = enemyDB.GetInfo(enemyIdx);
                GameObject prefab = enemyPrefabs[(int)info.enemyType];
                //instantiate GameObject
                Enemy enemy = Instantiate(prefab, unitParent).GetComponent<Enemy>();
                
                //set enemy rotation y randomly
                Vector3 rot = enemy.transform.rotation.eulerAngles;
                rot.y = UnityEngine.Random.Range(0, 360);
                enemy.transform.rotation = Quaternion.Euler(rot);
                
                //set enemy idx
                enemy.dataIndex = enemyIdx;
                enemy.gameObject.name = (i + 1) + " Enemy : " + enemyIdx;

                enemy.hexPosition = enemySpawnPointList[UnityEngine.Random.Range(0, enemySpawnPointList.Count)];
                enemySpawnPointList.Remove(enemy.hexPosition);
                
                units.Add(enemy);
            }
        }
        else if (GameManager.instance.CompareState(GameState.WORLD))
        {
            //find childeren unit
            var childCount = unitParent.childCount;
            units = new List<Unit>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                var child = unitParent.GetChild(i);
                if (child.TryGetComponent(out Unit unit))
                {
                    units.Add(unit);
                }
            }
        }

        foreach (Unit unit in units)
        {
            if (unit is Player p)
            {
                #region PLYAER PASSIVE SETUP
                
                var playerPassiveIndexList = GameManager.instance.playerPassiveIndexList;
                if (playerPassiveIndexList == null)
                {
                    Debug.LogError("GameManager.playerPassiveList is null");
                    return;
                }
                playerPassiveIndexList.Remove(0);
                var playerPassiveList = playerPassiveIndexList.Select(idx => passiveDB.GetPassive(idx, unit)).ToList();
                
                //remove passive that has null condition && world state
                if(GameManager.instance.CompareState(GameState.WORLD)) 
                    playerPassiveList.RemoveAll(pas => pas.GetConditionType()[0] is not ConditionType.Null);
                
                #endregion

                var activeList = GameManager.instance.playerActiveIndexList;
                foreach (var activeIdx in activeList)
                {
                    activeDB.AddAction(p, activeIdx);
                }
                p.AddComponent<IdleAction>();

                GameManager.instance.user.Stat.ResetModifier();
                p.SetUp(-1, "Player", GameManager.instance.user.Stat, 
                    weaponDB.Clone(GameManager.instance.playerWeaponIndex),
                    GameManager.instance.playerModel, playerPassiveList);
                if (GameManager.instance.CompareState(GameState.WORLD))
                {
                    p.hexTransform.position = GameManager.instance.runtimeWorldData.playerPosition;
                }
            }
            else if(unit is Enemy enemy)
            {
                EnemyData info = enemyDB.GetInfo(enemy.dataIndex);
                info.stat.ResetModifier();
                
                enemy.SetUp(enemy.dataIndex,
                    enemyDB.GetEnemyName(info.nameIndex), 
                    (UnitStat)info.stat.Clone(), 
                    weaponDB.Clone(info.weaponIndex), 
                    info.model, 
                    new List<Passive>()
                );
                // enemy.SetupAI();

                string path = AI_PATH + "AI " + info.btIndex;
                Debug.Log("AI Path : " + path);
                ScriptableObject ai = Resources.Load<ScriptableObject>(path);
                if(ai is null)
                {
                    Debug.LogError("AI is null, path : " + path);
                    throw new Exception();
                }
                
                Debug.Log("Found ScriptableObject : " + ai.name);
                if(ai is DecisionTree dt) {
                    enemy.SetupAI(dt);
                }
                else {
                    Debug.LogError("AI is not DecisionTree");
                    throw new Exception();
                }

                enemy.isVisible = false;

                _totalExp += info.rewardExp;
                rewardHandler.AddGold(info.rewardGold);
                rewardHandler.AddItem(info.rewardItem);
            }
            unit.onDead.AddListener(OnUnitDead);
            unit.onMoved.AddListener(OnUnitMoved);
            
            if(GameManager.instance.CompareState(GameState.COMBAT)) {
                CameraManager.instance.CreateUnitCamera(unit);
            }
            
            onUnitCreated.Invoke(unit);
        }
    }
    
    /// <summary>
    /// 현재 플레이어 객체를 가져옵니다.
    /// </summary>
    /// <returns>Player reference, or null</returns>
    public Player GetPlayer()
    {
        foreach (Unit unit in units)
        {
            if (unit is Player u) return u;
        }
        return null;
    }
    
    /// <summary>
    /// 특정 좌표에 존재하는 Unit을 가져옵니다.
    /// </summary>
    /// <param name="position">Hex 좌표</param>
    /// <returns>Unit Reference</returns>
    public Unit GetUnit(Vector3Int position)
    {
        if (units is null) return null;
        
        foreach (var unit in units)
        {
            if (unit.hexPosition == position) return unit;
        }

        return null;
    }

    public bool TryGetUnit(Vector3Int position, out Unit unit)
    {
        unit = null;
        if (units is null)
        {
            return false;
        }
        
        foreach (var u in units)
        {
            if (u.hexPosition == position)
            {
                unit = u;
                return true;
            }
        }

        return false;
    }

    public List<Unit> GetUnitListInRange(IEnumerable<Vector3Int> positions)
    {
        var result = new List<Unit>();
        if (units is null)
        {
            return result;
        }

        foreach (Vector3Int pos in positions)
        {
            if (TryGetUnit(pos, out Unit u))
            {
                result.Add(u);
            }
        }

        return result;
    }
    
    public List<Unit> GetUnitListInRange(Vector3Int start, int range)
    {
        var positions = Hex.GetCircleGridList(range, start);
        return GetUnitListInRange(positions);
    }

    public void OnUnitMoved(Unit unit)
    {
        onAnyUnitMoved?.Invoke(unit);
    }

    private void OnUnitDead(Unit unit)
    {
        Debug.Log($"OnUnitDeadCall : {unit.name}");
        RemoveUnit(unit);
        onAnyUnitDead.Invoke(unit);
    }

    private void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unit.onMoved.RemoveAllListeners();
    }

    public WeaponData GetWeaponData(int index) 
    {
        return weaponDB.GetData(index);
    }
    public EnemyData GetEnemyData(int index) 
    {
        return enemyDB.GetInfo(index);
    }

    public bool IsEnemyExist()
    {
        return units.OfType<Enemy>().Any();
    }
    
    public int GetEnemyCount()
    {
        return units.OfType<Enemy>().Count();
    }

    private void OnCombatFinish(bool isWin)
    {
        if (isWin)
        {
            LazyLevelHandler.ReservationExp(_totalExp);
            rewardHandler.ApplyReward();
        }
    }
}
