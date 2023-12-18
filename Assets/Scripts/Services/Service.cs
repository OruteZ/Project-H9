using System.Collections.Generic;
using UnityEngine;

public static class Service
{
    static private List<DamageFloaterManager> _floaters = new List<DamageFloaterManager>();
    static private Transform _rootCanvas;

    static Service()
    {
        var prefabObj = Resources.Load($"Prefab/Canvas/WorldTextCanvas") as GameObject;
        var worldCanvasObj = GameObject.Instantiate(prefabObj, Vector3.zero, Quaternion.identity);
        _rootCanvas = worldCanvasObj.transform;
        _rootCanvas.name = "WorldTextCanvas";
        GameObject.DontDestroyOnLoad(_rootCanvas);

        var commonDmg = new DamageFloaterManager();
        commonDmg.Init("Prefab/Damage Floater", _rootCanvas, 0.5f);
        var ciriticalDmg = new DamageFloaterManager();
        ciriticalDmg.Init("Prefab/Damage Floater Critical", _rootCanvas, 0.5f);

        _floaters.Add(commonDmg);
        _floaters.Add(ciriticalDmg);
    }
    public static void SetText(int index, string text, Vector3 position)
    {
        var t = _floaters[index].Set();
        var randValue = new Vector3(Random.value - 0.5f, Random.value, Random.value) * 2.0f;
      
        t.TMP.text = text;
        t.Instance.position = position + randValue;
    }

    public static void OnUpdated(float deltaTime)
    {
        foreach (var ins in _floaters)
        {
            ins.OnUpdated(deltaTime);
        }
    }
}
