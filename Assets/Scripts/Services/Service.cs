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
        var criticalDmg = new DamageFloaterManager();
        criticalDmg.Init("Prefab/Damage Floater Critical", _rootCanvas, 0.5f);
        var healDmg = new DamageFloaterManager();
        healDmg.Init("Prefab/Damage Floater Heal", _rootCanvas, 0.5f);

        _floaters.Add(commonDmg);
        _floaters.Add(criticalDmg);
        _floaters.Add(healDmg);

        UIManager.instance.onTakeDamaged.AddListener(DamagedText);
        UIManager.instance.onNonHited.AddListener(MissedText);
        UIManager.instance.onHealed.AddListener(DamagedText); // 피해를 입는 것과 같은 규칙 따름.
    }

    public static void MissedText(Unit unit)
    {
        Service.SetText(eDamageType.Type.Default, "MISS", unit.transform.position, 1.0f);
    }

    public static void DamagedText(Unit unit, int damage, eDamageType.Type type)
    {
        // 데미지에 비례하여 커지는 것은 게임의 진행 상황에 따라 "큰 데미지"의 기준이 바뀌므로, 지금은 임시로만
        // 15보다 높은 값에만 scale을 추가로 주겠음
        float scale = 1.0f;
        if (damage > 15) scale = 1.5f;

        Service.SetText(type, damage.ToString(), unit.transform.position, scale);
    }

    /// <summary>
    /// Type에 따라 어떤 damageFloater를 사용하는지는 생성자의 List 들어가는 프리팹 이름 확인
    /// </summary>
    public static void SetText(eDamageType.Type type, string text, Vector3 position, float scale = 1.0f)
    {
        DamageFlaoterWrapper floater = null;
        if (type.HasFlag(eDamageType.Type.Critical))
        {
            floater = _floaters[1].Set();
        }
        else if (type.HasFlag(eDamageType.Type.Burned))
        {
            floater = _floaters[0].Set();
        }
        else if (type.HasFlag(eDamageType.Type.Blooded))
        {
            floater = _floaters[0].Set();
        }
        else if (type.HasFlag(eDamageType.Type.Heal))
        {
            floater = _floaters[2].Set();
        }
        else
        {
            floater = _floaters[0].Set();
        }
        var randValue = new Vector3(Random.value - 0.5f, Random.value, Random.value) * 2.0f;
      
        floater.TMP.text = text;
        floater.Instance.position = position + randValue;
        floater.Instance.localScale = Vector3.one * scale * floater.ScaleStart;
    }

    public static void OnUpdated(float deltaTime)
    {
        foreach (var ins in _floaters)
        {
            ins.OnUpdated(deltaTime);
        }
    }
}
