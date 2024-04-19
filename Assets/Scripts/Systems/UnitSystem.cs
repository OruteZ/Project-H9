using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UnitSystem : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDB;
    [SerializeField] private EnemyDatabase enemyDB;
    [SerializeField] private PassiveDatabase passiveDB;
    [SerializeField] private ActiveDatabase activeDB;
    [SerializeField] private BehaviourTreeDatabase aiDB;
    [SerializeField] private LinkDatabase linkDB;
    
    public List<Unit> units;
    public UnityEvent<Unit> onAnyUnitMoved;
    public UnityEvent<Vector3> onPlayerMoved;
    public UnityEvent<Unit> onAnyUnitDead;
    
    [SerializeField] private Transform unitParent;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    
    private int _totalExp;
    public CombatRewardHelper rewardHelper;

    private CustomOutline.Mode[] outlineMode = { CustomOutline.Mode.NULL, CustomOutline.Mode.OutlineAll, CustomOutline.Mode.SilhouetteOnly };
    private int modeIndex = 0;
    private void Update()
    {
        if (Input.GetKeyDown(HotKey.changeOutlineModeKey)) 
        {
            Player player = GetPlayer();
            if (player is null) return;
            player.TryGetComponent(out CustomOutline outline);
            if (outline is null) 
            {
                outline = player.AddComponent<CustomOutline>();
                outline.OutlineColor = Color.red;
            }

            if (++modeIndex >= outlineMode.Length) modeIndex = 0;
            outline.OutlineMode = outlineMode[modeIndex];
        }
    }

    /// <summary>
    /// 유닛을 생성하고, 유닛의 데이터를 초기화합니다.
    /// </summary>
    public void SetUpUnits()
    {
        _totalExp = 0;
        rewardHelper = new CombatRewardHelper(GameManager.instance.itemDatabase);

        //get all link data and instantiate enemy
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            var linkDataIdx = GameManager.instance.GetLinkIndex();
            var linkData = linkDB.GetData(linkDataIdx);
            var enemyCount = linkData.combatEnemy.Length;

            var mapSpawnData = GameManager.instance.GetStageData();
            if (!mapSpawnData.TryGetEnemyPoints(linkDataIdx, out var enemySpawnPoints))
            {
                Debug.LogError("해당 링크의 Enemy Spawn Point is null");
                return;
            }

            if (!mapSpawnData.TryGetPlayerPoint(linkDataIdx, out var playerSpawnPoint))
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
                return;
            }
            
            for (int i = 0; i < enemyCount; i++)
            {
                var enemyIdx = linkData.combatEnemy[i];
                //instantiate GameObject
                var enemy = Instantiate(enemyPrefab, unitParent).GetComponent<Enemy>();
                
                //set enemy rotation y randomly
                var rot = enemy.transform.rotation.eulerAngles;
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
        else if (GameManager.instance.CompareState(GameState.World))
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

        foreach (var unit in units)
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
                
                var playerPassiveList = playerPassiveIndexList.Select(idx => passiveDB.GetPassive(idx, unit)).ToList();
                
                //remove passive that has null condition && world state
                if(GameManager.instance.CompareState(GameState.World)) 
                    playerPassiveList.RemoveAll(pas => pas.GetConditionType() is not ConditionType.Null);
                #endregion

                var activeList = GameManager.instance.playerActiveIndexList;
                foreach (var activeIdx in activeList)
                {
                    activeDB.GetAction(p, activeIdx);
                }
                p.AddComponent<IdleAction>();

                GameManager.instance.playerStat.ResetModifier();
                p.SetUp(-1, "Player", GameManager.instance.playerStat, 
                    weaponDB.Clone(GameManager.instance.PlayerWeaponIndex),
                    GameManager.instance.playerModel, playerPassiveList);
                if (GameManager.instance.CompareState(GameState.World))
                {
                    p.hexTransform.position = GameManager.instance.playerWorldPos;
                }
            }
            else if(unit is Enemy enemy)
            {
                var info = enemyDB.GetInfo(enemy.dataIndex);
                info.stat.ResetModifier();
                
                enemy.SetUp(enemy.dataIndex, "Enemy", (UnitStat)info.stat.Clone(), weaponDB.Clone(info.weaponIndex), info.model, new List<Passive>());
                enemy.SetupAI(aiDB.GetTree(info.btIndex));
                enemy.isVisible = false;

                _totalExp += info.rewardExp;
                rewardHelper.AddGold(info.rewardGold);
                rewardHelper.AddItem(info.rewardItem);
            }
            unit.onDead.AddListener(OnUnitDead);
            unit.onMoved.AddListener(OnUnitMoved);
            
            if(GameManager.instance.CompareState(GameState.Combat)) {
                CameraManager.instance.CreateUnitCamera(unit);
            }
        }
    }

    public bool IsCombatFinish(out bool hasPlayerWin)
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            Debug.LogError("Wrong function Call : check finish combat in world scene");
            throw new NotSupportedException();
        }

        hasPlayerWin = false;
        if (GetPlayer() is null) return true;
        if (GetPlayer().hp <= 0) return true;
        
        if (units.Count == 1 && units[0] is Player)
        {
            hasPlayerWin = true;
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// 현재 플레이어 객체를 가져옵니다.
    /// </summary>
    /// <returns>Player reference, or null</returns>
    public Player GetPlayer()
    {
        foreach (var unit in units)
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

        foreach (var pos in positions)
        {
            if (TryGetUnit(pos, out var u))
            {
                result.Add(u);
            }
        }

        return result;
    }
    public List<Unit> GetUnitListInRange(Vector3Int start, int range)
    {
        var positions = Hex.GetCircleGridList(range, start);
        var result = new List<Unit>();
        if (units is null)
        {
            return result;
        }

        foreach (var pos in positions)
        {
            if (TryGetUnit(pos, out var u))
            {
                result.Add(u);
            }
        }

        return result;
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
        
        if (IsCombatFinish(out var playerWin))
        {
            if (playerWin)
            {
                LevelSystem.ReservationExp(_totalExp);
                rewardHelper.ApplyReward();
            }
            FieldSystem.onCombatFinish.Invoke(playerWin);
        }
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

    public bool isEnemyExist() 
    {
        foreach (var unit in units) 
        {
            if (unit is Enemy) return true;
        }
        return false;
    }
}
