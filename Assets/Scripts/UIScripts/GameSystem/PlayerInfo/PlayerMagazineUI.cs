using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMagazineUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _magazineTooltip;

    private int MAGAZINE_SIZE = 6;
    private int MAGAZINE_COUNT = 7;
    void Start()
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip?.SetActive(false);
    }
    public void SetMagazineUI(bool isOnlyDisplayMaxMagazine)
    {
        if (transform.childCount != MAGAZINE_COUNT) 
        {
            Debug.LogError("배치된 UI 요소 개수와 코드 상 상수가 다릅니다.");
            return;
        }
        int maxBullet;
        int curBullet;
        int flickerCnt;
        if (isOnlyDisplayMaxMagazine)
        {
            maxBullet = GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex).weaponAmmo;
            curBullet = maxBullet;
            flickerCnt = 0;
        }
        else 
        {
            maxBullet = FieldSystem.unitSystem.GetPlayer().weapon.maxAmmo;
            curBullet = FieldSystem.unitSystem.GetPlayer().weapon.currentAmmo;
            flickerCnt = UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedMagUsage;
        }

        int nonFlickerCnt = curBullet - flickerCnt;
        for (int i = 0; i < transform.childCount; i++) 
        {
            int existCnt = MAGAZINE_SIZE;
            int filledCnt = MAGAZINE_SIZE;
            flickerCnt = 0;
            if (curBullet <= MAGAZINE_SIZE)
            {
                filledCnt = curBullet;
                existCnt = maxBullet;
            }
            if (nonFlickerCnt <= MAGAZINE_SIZE)
            {
                flickerCnt = filledCnt - nonFlickerCnt;
            }

            transform.GetChild(i).GetComponent<PlayerMagazineUIElement>().SetPlayerMagUIElement(existCnt, filledCnt, flickerCnt);

            maxBullet -= MAGAZINE_SIZE;
            curBullet -= MAGAZINE_SIZE;
            nonFlickerCnt -= MAGAZINE_SIZE;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip?.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_magazineTooltip == null) return;
        _magazineTooltip?.SetActive(false);
    }
}
