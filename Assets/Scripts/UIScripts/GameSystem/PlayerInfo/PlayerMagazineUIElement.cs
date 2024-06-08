using Mono.Cecil;
using NSubstitute.Exceptions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagazineUIElement : UIElement
{
    [SerializeField] private Sprite _bulletSprite;
    [SerializeField] private Sprite _goldenBulletSprite;

    private int _maxSize = -1;
    private List<PlayerBulletUIElement> _elements = new List<PlayerBulletUIElement>();

    private void Awake()
    {
        _maxSize = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            _elements.Add(transform.GetChild(i).GetComponent<PlayerBulletUIElement>());
        }
    }

    public void SetGoldenBullet(int idx)
    {
        if (idx < 0 || _elements.Count <= idx)
            Debug.LogError($"HUD magazine UI => Golden Bullet index is incorrect. ({gameObject.name} idx:{idx})");
        _elements[idx].GetComponent<Image>().sprite = _goldenBulletSprite;
    }

    public void Reload(int maxAmmo, int curAmmo, int expectedCnt = 0)
    {
        if (maxAmmo < 0 || _elements.Count <= maxAmmo)
            Debug.LogError($"HUD magazine UI => SetMaxSize()'s size is incorrect. ({gameObject.name} maxAmmo:{maxAmmo})");
        
        // use bullets
        for (int i = 0; i <= curAmmo; i++)
        {
            _elements[i].Fill();
            _elements[i].GetComponent<Image>().sprite = _bulletSprite;
        }

        for (int i = curAmmo + 1; i < maxAmmo; i++)
        {
            _elements[i].Empty();
            _elements[i].GetComponent<Image>().sprite = _bulletSprite;
        }

        // unuse bullets (upper maxAmmo)
        for (int i = maxAmmo; i < _elements.Count; i++)
        {
            _elements[i].Hide();
        }

        // flicker
        for (int i = curAmmo; 0 <= i; i--)
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
    /*
    public void SetPlayerMagUIElement(int existCnt, int filledCnt, int flickerCnt) 
    {
        bool isExist = true;
        bool isFilled = true;
        bool isFlickering = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i >= filledCnt - flickerCnt) isFlickering = true;
            if (i >= filledCnt) isFilled = false;
            if (i >= existCnt) isExist = false;
            transform.GetChild(i).GetComponent<PlayerBulletUIElement>().SetBulletUIElement(isExist, isFilled, isFlickering);
        }
    }
    */
}
