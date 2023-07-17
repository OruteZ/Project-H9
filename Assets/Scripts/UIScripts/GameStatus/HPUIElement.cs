using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUIElement : MonoBehaviour
{
    private bool _isFill;

    private void Start()
    {
        _isFill = false;
    }
    public void FillUI() 
    {
        GetComponent<Image>().color = new Color(1, 0, 0, 1);
        _isFill = true;
    }

    public void EmptyUI()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        _isFill = false;
    }
}
