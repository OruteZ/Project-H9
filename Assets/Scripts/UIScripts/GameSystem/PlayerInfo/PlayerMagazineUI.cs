using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMagazineUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _magazineTooltip;

    [SerializeField] private PlayerRevolverMagazineUIElement _revolvers;
    [SerializeField] private PlayerMagazineUIElement _shotguns;
    [SerializeField] private PlayerMagazineUIElement _repeater;

    private float _revolverRotation = 0;

    void Start()
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip?.SetActive(false);
        _revolvers.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 359.99f);
    }
    public void SetMagazineUI(bool isOnlyDisplayMaxMagazine)
    {
        int maxAmmo;
        Magazine magazine;
        int flickerCnt;

        if (isOnlyDisplayMaxMagazine)
        {
            maxAmmo = GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex).weaponAmmo;
            magazine = new Magazine(maxAmmo);
            flickerCnt = 0;
        }
        else
        {
            maxAmmo = FieldSystem.unitSystem.GetPlayer().weapon.maxAmmo;
            magazine = FieldSystem.unitSystem.GetPlayer().weapon.magazine;
            flickerCnt = UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedMagUsage;
        }
        maxAmmo = GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex).weaponAmmo;

        if (_revolvers.gameObject.activeSelf == true)
            _revolvers.gameObject.SetActive(false);
        if (_shotguns.gameObject.activeSelf == true)
            _shotguns.gameObject.SetActive(false);
        if (_repeater.gameObject.activeSelf == true)
            _repeater.gameObject.SetActive(false);

        ItemType weaponType = FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType();
        if (weaponType == ItemType.Revolver)
        {
            _revolvers.gameObject.SetActive(true);
            _revolvers.Reload(maxAmmo, magazine, flickerCnt);
            _revolverRotation = 359.99f - (360 / maxAmmo) * (maxAmmo - magazine.bullets.Count);
        }
        if (weaponType == ItemType.Shotgun)
        {
            _shotguns.gameObject.SetActive(true);
            _shotguns.Reload(maxAmmo, magazine, flickerCnt);
        }
        if (weaponType == ItemType.Repeater)
        {
            _repeater.gameObject.SetActive(true);
            _repeater.Reload(maxAmmo, magazine, flickerCnt);
        }
    }
    private void Update()
    {
        if (_revolvers is null || !_revolvers.gameObject.activeSelf) return;
            float rotation = _revolvers.GetComponent<RectTransform>().eulerAngles.z;
        if (LerpCalculation.CalculateLerpValue(ref rotation, _revolverRotation, 10f))
        {
            _revolvers.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, rotation);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip.GetComponent<PlayerMainStatTooltip>().SetPlayerMainStatTooltip(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip?.SetActive(false);
    }
}
