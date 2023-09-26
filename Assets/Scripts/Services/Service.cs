using System.Collections.Generic;
using UnityEngine;

public static class Service
{
    static private List<DamageFloaterManager> _floaters = new List<DamageFloaterManager>();
    static private GameObject _rootCanvas;
    static private RectTransform _rootCanvasRect;
    static private Camera _camera;

    static Service()
    {
        _rootCanvas = GameObject.Find("Effect Canvas");
        _rootCanvasRect = _rootCanvas.GetComponent<RectTransform>();
        _rootCanvasRect.anchoredPosition = Vector2.zero;
        var common = new DamageFloaterManager();
        common.Init("Prefab/Damage Floater", _rootCanvas.transform, 1.5f);
        _floaters.Add(common);
        SetCamera(Camera.main);

    }
    public static void SetCamera(Camera target)
    {
        _camera = target;
    }

    public static void SetText(string text, Vector3 position)
    {
        Vector2 viewportPosition = _camera.WorldToViewportPoint(position);
        var sizeDelta = _rootCanvasRect.sizeDelta;
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
            ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));

        var t = _floaters[0].Set();
        t.TMP.text = text;
        t.Instance.anchoredPosition = worldObjectScreenPosition;
    }

    public static void OnUpdated(float deltaTime)
    {
        foreach (var ins in _floaters)
        {
            ins.OnUpdated(deltaTime);
        }
    }
}
