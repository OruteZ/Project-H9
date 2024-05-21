using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfoUI : UIElement
{
    [SerializeField] private GameObject _enemyPortrait;
    [SerializeField] private GameObject _enemyWeapon;
    [SerializeField] private GameObject[] _enemyStats = new GameObject[4];
    [SerializeField] private GameObject _enemyStatTooltip;
    [SerializeField] private GameObject _enemyWeaponTooltip;

    private void Awake()
    {
        CloseStatTooltip();
    }

    public void OpenInventoryTooltip(GameObject ui, Vector3 pos)
    {
        _enemyWeaponTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(ui, pos);
    }
    public void CloseInventoryTooltip()
    {
        _enemyWeaponTooltip.GetComponent<InventoryUITooltip>().CloseUI();
    }
    internal void OpenStatTooltip(Vector3 pos, string name)
    {
        _enemyStatTooltip.GetComponent<RectTransform>().position = pos;
        _enemyStatTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        _enemyStatTooltip.SetActive(true);
    }
    internal void CloseStatTooltip()
    {
        _enemyStatTooltip.GetComponent<RectTransform>().position = Vector3.zero;
        _enemyStatTooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        _enemyStatTooltip.SetActive(false);
    }
    public void SetEnemyInfoUI(Enemy enemy)
    {
        _enemyPortrait.GetComponent<Image>().sprite = Resources.Load("UnitCapture" + enemy.unitName) as Sprite;
        SetEnemyStatText(enemy.stat);
        SetEnemyWeapon(enemy.weapon);
    }

    private void SetEnemyStatText(UnitStat enemyStat)
    {
        _enemyStats[0].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement("Health Point", enemyStat.maxHp);
        _enemyStats[1].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement("Action Point", enemyStat.maxActionPoint);
        _enemyStats[2].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement("Concentration", enemyStat.concentration);
        _enemyStats[3].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement("Speed", enemyStat.speed);
    }
    private void SetEnemyWeapon(Weapon weapon)
    {
        ItemData iData = GameManager.instance.itemDatabase.GetItemData(weapon.nameIndex);
        WeaponItem item = new WeaponItem(iData);
        _enemyWeapon.GetComponent<InventoryUIEnemyWeapon>().SetInventoryUIElement(item);
    }
}
