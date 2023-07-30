using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUIElement : MonoBehaviour
{
    private bool _isFill;

    private void Start()
    {
        _isFill = false;
    }
    public void FillUI() 
    {
        if (!_isFill)
        {
            GetComponent<Image>().color = new Color(1, 0, 0, 1);
            _isFill = true;
        }
    }

    public void EmptyUI()
    {
        if (_isFill)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
            _isFill = false;
        }
    }
}
