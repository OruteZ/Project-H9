using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 적 체력바 표시와 관련된 기능을 구현한 클래스
/// </summary>
public class EnemyHpUI : UISystem
{
    [SerializeField] private GameObject _enemyHpBarContainer;

    private EnemyHpUIPool _pool = null;

    [HideInInspector] public UnityEvent<GameObject> onEnemyHpDeleted;

    private void Start()
    {
        UIManager.instance.onActionChanged.AddListener(SetEnemyHpBars);
        onEnemyHpDeleted.AddListener((u) => ClearEnemyHpUIElement(u));
        if (_pool == null)
        {
            _pool = new EnemyHpUIPool();
            _pool.Init("Prefab/Enemy Hp Slider", _enemyHpBarContainer.transform, 0);
        }
    }

    /// <summary>
    /// 적 체력바 UI를 설정합니다.
    /// 액션이 시작될 때, 액션이 끝날 때, 액션을 선택할 때 실행됩니다.
    /// </summary>
    public void SetEnemyHpBars()
    {
        if (GameManager.instance.CompareState(GameState.World)) return;
        List<Unit> units = FieldSystem.unitSystem.units;
        List<Enemy> newEnemies = new List<Enemy>();
        foreach (Unit unit in units)
        {
            if (unit is Enemy)
            {
                if (_pool.FindByEnemy((Enemy)unit) == null)
                {
                    newEnemies.Add((Enemy)unit);
                }
            }
        }

        for (int i = 0; i < newEnemies.Count; i++) 
        {
            var wrapper = _pool.Set();
            wrapper.Instance.GetComponent<EnemyHpUIElement>().InitEnemyHpUI(newEnemies[i]);
        }
        _pool.Update();
    }
    private void ClearEnemyHpUIElement(GameObject ui)
    {
        _pool.Reset(ui);
    }
}
