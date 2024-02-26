using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUIElement : UIElement
{
    [SerializeField] private GameObject _bulletOutline;
    [SerializeField] private GameObject _bulletEmpty;
    [SerializeField] private GameObject _bulletFill;

    public void SetBulletUIElement(bool isExist, bool isFilled, bool isFlickering)
    {
        _bulletOutline.SetActive(isExist);
        _bulletEmpty.SetActive(isExist);
        _bulletFill.SetActive(isFilled);
        if (isFilled && isFlickering)
        {
            _bulletFill.GetComponent<Animator>().enabled = true;
            _bulletFill.GetComponent<Animator>().Rebind();
            _bulletFill.GetComponent<Animator>().Play("Fade In & Out Effect");
        }
        else
        {
            _bulletFill.GetComponent<Animator>().Rebind();
            _bulletFill.GetComponent<Animator>().enabled = false;
        }
    }
}
