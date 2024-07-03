using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGameObjectButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;

    public void ActiveTrue()
    {
        if (_target.activeSelf == false)
            _target.SetActive(true);
    }

    public void ActiveFalse()
    {
        if (_target.activeSelf == true)
            _target.SetActive(false);
    }
}
