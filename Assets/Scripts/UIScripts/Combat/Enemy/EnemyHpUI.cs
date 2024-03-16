using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 체력바 표시와 관련된 기능을 구현한 클래스
/// </summary>
public class EnemyHpUI : UISystem
{
    [SerializeField] private GameObject _enemyHpBarPrefab;
    [SerializeField] private GameObject _enemyHpBarContainer;

    private List<GameObject> _enemyHpBars;

    private List<Enemy> _enemies = new List<Enemy>();

    private void Awake()
    {

        _enemyHpBars = new List<GameObject>();
        EnemyHpBarObjectPooling(10);
        UIManager.instance.onActionChanged.AddListener(() => SetEnemyHpBars());
    }

    /// <summary>
    /// 적 체력바 UI를 설정합니다.
    /// 액션이 시작될 때, 액션이 끝날 때, 액션을 선택할 때 실행됩니다.
    /// </summary>
    public void SetEnemyHpBars()
    {
        List<Unit> units = FieldSystem.unitSystem.units;
        _enemies = new List<Enemy>();
        foreach (Unit unit in units)
        {
            if (unit is Enemy)
            {
                _enemies.Add((Enemy)unit);
            }
        }

        //Debug.Log("적 개체 수:" + _enemies.Count);

        if (_enemies.Count > _enemyHpBars.Count) 
        {
            EnemyHpBarObjectPooling(10);
        }
        for (int i = 0; i < _enemyHpBars.Count; i++) 
        {
            if (i < _enemies.Count)
            {
                _enemyHpBars[i].GetComponent<EnemyHpUIElement>().SetEnemyHpUI(_enemies[i]);
            }
        }
    }
    private void InitEnemyHpUIs()
    {
        foreach (GameObject hpBar in _enemyHpBars)
        {
            hpBar.GetComponent<EnemyHpUIElement>().ClearEnemyHpUI();
        }
    }
    private void EnemyHpBarObjectPooling(int length)
    {
        for (int i = 0; i < length; i++)
        {
            GameObject ui = Instantiate(_enemyHpBarPrefab, Vector3.zero, Quaternion.identity, _enemyHpBarContainer.transform);
            _enemyHpBars.Add(ui);
        }
        InitEnemyHpUIs();
    }
}
