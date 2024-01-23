using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUIElement : UIElement
{
    [SerializeField] private GameObject _bulletEmpty;
    [SerializeField] private GameObject _bulletFill;

    public void SetBulletUIElement(bool isExist, bool isFilled)
    {
        _bulletEmpty.SetActive(isExist);
        _bulletFill.SetActive(isFilled);
    }
}
