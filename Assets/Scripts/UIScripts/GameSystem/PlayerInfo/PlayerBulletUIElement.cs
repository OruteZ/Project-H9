using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUIElement : UIElement
{
    [SerializeField] private Transform _bulletFill;

    public void Fill()
    {
        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        if (_bulletFill.gameObject.activeSelf == false)
            _bulletFill.gameObject.SetActive(true);
    }

    public void Empty()
    {
        if (this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        if (_bulletFill.gameObject.activeSelf == true)
        {
            _bulletFill.gameObject.SetActive(false);
            StopFlick();
        }
    }

    // Hide included edge or background.
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Flick()
    {
        _bulletFill.GetComponent<Animator>().enabled = true;
        _bulletFill.GetComponent<Animator>().Rebind();
        _bulletFill.GetComponent<Animator>().Play("Fade In & Out Effect");
    }

    public void StopFlick()
    {
        _bulletFill.GetComponent<Animator>().Rebind();
        _bulletFill.GetComponent<Animator>().enabled = false;
    }
}
