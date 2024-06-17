using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerApUIElement : UIElement
{
    [SerializeField] private GameObject _apEmpty;
    [SerializeField] private GameObject _apFill;

    void Start()
    {
        _apEmpty.SetActive(false);
        _apFill.SetActive(false);
    }

    public void SetApUIElement(bool isExist, bool isFilled, bool isFlickering)
    {
        _apEmpty.SetActive(isExist);
        _apFill.SetActive(isFilled);
        if (isFilled && isFlickering)
        {
            _apFill.GetComponent<Animator>().enabled = true;
            _apFill.GetComponent<Animator>().Rebind();
            _apFill.GetComponent<Animator>().Play("Fade In & Out Effect");
        }
        else
        {
            _apFill.GetComponent<Animator>().Rebind();
            if (isFilled) _apFill.GetComponent<Animator>().Update(1);
            _apFill.GetComponent<Animator>().enabled = false;
        }
    }
}
