using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimEffect : MonoBehaviour
{
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetAimEffect(float hitRate)
    {
        _image.rectTransform.rect.Set(0, 0, (hitRate * 100), (hitRate * 100));
    }
}
