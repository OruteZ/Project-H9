using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerApUIElement : UIElement
{
    [SerializeField] private GameObject _apEmpty;
    [SerializeField] private GameObject _apFill;
    // Start is called before the first frame update
    void Start()
    {
        _apEmpty.SetActive(false);
        _apFill.SetActive(false);
    }

    public void SetApUIElement(bool isExist, bool isFilled)
    {
        _apEmpty.SetActive(isExist);
        _apFill.SetActive(isFilled);
    }
}
