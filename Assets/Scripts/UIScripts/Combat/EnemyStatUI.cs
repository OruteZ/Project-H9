using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyStatUI : UISystem
{
    [SerializeField] private GameObject _enemyStatWindow;

    [SerializeField] private GameObject _enemyStatText;
    [SerializeField] private GameObject _weaponStatText1Contents;
    [SerializeField] private GameObject _weaponStatText2Contents;
    [SerializeField] private GameObject _weaponStatText3Name;
    [SerializeField] private GameObject _weaponStatText3Contents;

    private const float WINDOW_X_POSITION_CORRECTION = 400;

    public override void CloseUI()
    {
        base.CloseUI();
        CloseEnemyStatUI();
    }

    public override void OpenPopupWindow()
    {
        UIManager.instance.previousLayer = 3;
        _enemyStatWindow.SetActive(true);
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.previousLayer = 1;
        CloseEnemyStatUI();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Player player = FieldSystem.unitSystem.GetPlayer();
            if (player is null || player.GetSelectedAction().GetActionType() is not ActionType.Idle) return;
            Vector3Int enemyPos;
            if (GetMouseClickObject(out enemyPos))
            {
                Debug.Log("Click");
                SetEnemyStatUI((Enemy)FieldSystem.unitSystem.GetUnit(enemyPos));
            }
        }
    }
    public void SetEnemyStatUI(Enemy enemy) 
    {
        OpenPopupWindow();
        Vector3 enemyPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
        enemyPos.x -= WINDOW_X_POSITION_CORRECTION;
        _enemyStatWindow.transform.position = enemyPos;
        SetCharacterStatText(enemy.GetStat());
        SetWeaponStatText(enemy.weapon);
    }
    private void SetCharacterStatText(UnitStat enemyStat)
    {
        string text = enemyStat.maxHp.ToString() + '\n' +
                      enemyStat.concentration.ToString() + '\n' +
                      enemyStat.sightRange.ToString() + '\n' +
                      enemyStat.speed.ToString() + '\n' +
                      enemyStat.actionPoint.ToString() + '\n' +
                      enemyStat.additionalHitRate.ToString() + '\n' +
                      enemyStat.criticalChance.ToString();

        _enemyStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(Weapon weapon)
    {
        string text1 = /*weapon.weaponName +*/'\n' +
                       weapon.weaponDamage.ToString();
        _weaponStatText1Contents.GetComponent<TextMeshProUGUI>().text = text1;

        string text2 = weapon.currentAmmo.ToString() + " / " + weapon.maxAmmo.ToString() + '\n' +
                       weapon.weaponRange.ToString();
        _weaponStatText2Contents.GetComponent<TextMeshProUGUI>().text = text2;



        string text3 = "", text4 = "";
        float hitRate = weapon.hitRate;
        float criChance = weapon.criticalChance;
        float criDamage = weapon.criticalDamage;
        if (hitRate != 0)
        {
            text3 += "Hit Rate:" + '\n';
            text4 += hitRate.ToString() + '\n';
        }
        if (criChance != 0)
        {
            text3 += "Critical Chance:" + '\n';
            text4 += criChance.ToString() + '\n';
        }
        if (criDamage != 0)
        {
            text3 += "Critical Damage:" + '\n';
            text4 += criDamage.ToString() + '\n';
        }
        _weaponStatText3Name.GetComponent<TextMeshProUGUI>().text = text3;
        _weaponStatText3Contents.GetComponent<TextMeshProUGUI>().text = text4;
    }
    public void CloseEnemyStatUI()
    {
        _enemyStatWindow.SetActive(false);
    }
    private static bool GetMouseClickObject(out Vector3Int pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Enemy")))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            pos = enemy.hexPosition;
            return true;
        }

        pos = Vector3Int.zero;
        return false;
    }
}
