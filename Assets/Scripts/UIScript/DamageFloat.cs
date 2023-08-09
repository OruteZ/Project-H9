using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageFloat : MonoBehaviour
{
    private Vector3 _worldPosition;
    private float _duration;
    private float _startTime;

    private RectTransform _parentCanvasRectTsf;
    public float randomScale;

    private Camera _cam;

    private Camera cam
    {
        get
        {
            if (_cam == null) _cam = Camera.main;
            return _cam;
        }
    }

    private TextMeshProUGUI _textMeshPro;
    // Update is called once per frame
    public void Awake()
    {
        _cam = Camera.main;
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _parentCanvasRectTsf = GameObject.Find("Effect Canvas").GetComponent<RectTransform>();
        
        transform.SetParent(_parentCanvasRectTsf);
    }

    public void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {
        var deltaTime = Time.time - _startTime;
        if(deltaTime > _duration) Destroy(gameObject);

        UpdatePosition();
    }

    public void SetPosition(Vector3 position, float duration)
    {
        _worldPosition = position + new Vector3(Random.value, Random.value,Random.value) * randomScale;
        _duration = duration;
    }

    public void SetValue(int damage)
    {
        if (_textMeshPro is null) _textMeshPro = GetComponent<TextMeshProUGUI>();
        _textMeshPro.text = damage.ToString();
    }

    private void UpdatePosition()
    {
        Vector2 viewportPosition = cam.WorldToViewportPoint(_worldPosition);
        var sizeDelta = _parentCanvasRectTsf.sizeDelta;
            
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
            ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
        ((RectTransform)gameObject.transform).anchoredPosition = worldObjectScreenPosition;
    }
}
