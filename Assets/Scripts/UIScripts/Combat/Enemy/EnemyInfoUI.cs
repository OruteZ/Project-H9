using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfoUI : UIElement
{
    [SerializeField] private GameObject _enemyCountText;
    [SerializeField] private GameObject _enemyPortrait;
    [SerializeField] private GameObject _enemyLevelText;
    [SerializeField] private GameObject _enemyWeapon;
    [SerializeField] private GameObject[] _enemyStats = new GameObject[4];
    [SerializeField] private GameObject _enemyStatTooltip;
    [SerializeField] private GameObject _enemyWeaponTooltip;

    [SerializeField] private Sprite _defaultEnemyImage;

    private void Awake()
    {
        CloseStatTooltip();
        UIManager.instance.onTSceneChanged.AddListener((gs) => { CloseStatTooltip(); });
    }

    public void OpenInventoryTooltip(GameObject ui, Vector3 pos, bool isAPVisible = false)
    {
        _enemyWeaponTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(ui, pos, isAPVisible);
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


    public void SetEnemyInfoUI(EnemyData data, int count)
    {
        string countText = count + "x";
        if (count == 0) countText = "";
        _enemyCountText.GetComponent<TextMeshProUGUI>().text = countText;
        _enemyLevelText.GetComponent<TextMeshProUGUI>().text = data.level.ToString();
        Texture2D enemyTexture = Resources.Load("UnitCapture/" + data.modelName) as Texture2D;
        Sprite enemySpr = Sprite.Create(enemyTexture, new Rect(0, 0, enemyTexture.width, enemyTexture.height), new Vector2(0.5f, 0.5f));
        if (enemySpr == null) enemySpr = _defaultEnemyImage;
        _enemyPortrait.GetComponent<Image>().sprite = enemySpr;
        SetEnemyStatText(data.stat);
        SetEnemyWeapon(data.weaponIndex);
    }
    public void SetEnemyInfoUI(EnemyData data, int count, GameObject statTooltip, GameObject weaponTooltip)
    {
        SetEnemyInfoUI(data, count);
        _enemyStatTooltip = statTooltip;
        _enemyWeaponTooltip = weaponTooltip;
    }

    private void SetEnemyStatText(UnitStat enemyStat)
    {
        _enemyStats[0].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement(UIManager.instance.UILocalization[26], enemyStat.maxHp);
        _enemyStats[1].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement(UIManager.instance.UILocalization[27], enemyStat.maxActionPoint);
        _enemyStats[2].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement(UIManager.instance.UILocalization[29], enemyStat.concentration);
        _enemyStats[3].GetComponent<EnemyStatUIElement>().SetEnemyStatUIElement(UIManager.instance.UILocalization[22], enemyStat.speed);
    }
    private void SetEnemyWeapon(int weaponIndex)
    {
        ItemData iData = GameManager.instance.itemDatabase.GetItemData(weaponIndex);
        WeaponItem item = new (iData);
        _enemyWeapon.GetComponent<InventoryUIEnemyWeapon>().SetInventoryUIElement(item);
    }
}
