using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRevolverMagazineUIElement : PlayerMagazineUIElement
{
    private const float RADIUS = 25.0f;

    public override void Reload(int maxAmmo, Magazine magazine, int expectedCnt = 0)
    {
        if (maxAmmo < 0 || _elements.Count <= maxAmmo)
            Debug.LogError($"HUD magazine UI => SetMaxSize()'s size is incorrect. ({gameObject.name} maxAmmo:{maxAmmo})");

        for (int i = 0; i < _elements.Count; i++)
        {
            float angle = i * (360.0f / maxAmmo);
            angle *= Mathf.Deg2Rad;
            _elements[i].GetComponent<RectTransform>().localPosition = new Vector3(-RADIUS * Mathf.Sin(angle), RADIUS * Mathf.Cos(angle), 0);
        }


        for (int i = 0; i < maxAmmo - magazine.bullets.Count; i++)
        {
            _elements[i].Empty();
            _elements[i].transform.GetChild(1).GetComponent<Image>().sprite = _bulletSprite;
        }

        int bulletCnt = 0;
        for (int i = maxAmmo - magazine.bullets.Count; i < maxAmmo; i++)
        {
            _elements[i].Fill();
            if (GameManager.instance.CompareState(GameState.World) || !magazine.bullets[bulletCnt].isGoldenBullet)
            {
                _elements[i].transform.GetChild(1).GetComponent<Image>().sprite = _bulletSprite;
            }
            else
            {
                _elements[i].transform.GetChild(1).GetComponent<Image>().sprite = _goldenBulletSprite;
            }
            bulletCnt++;
        }

        // unuse bullets (upper maxAmmo)
        for (int i = maxAmmo; i < _elements.Count; i++)
        {
            _elements[i].Hide();
        }

        if (prevCnt != maxAmmo) 
        {
            prevCnt = maxAmmo;
            return;
        }
        // flicker
        for (int i = 0; i<magazine.bullets.Count; i++)
        {
            if (0 != expectedCnt)
            {
                _elements[i].Flick();
                expectedCnt--;
            }
            else
            {
                _elements[i].StopFlick();
            }
        }

    }
}
