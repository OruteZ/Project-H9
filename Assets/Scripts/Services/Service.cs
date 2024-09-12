using System.Collections.Generic;
using UnityEngine;

public static class Service
{
    static private List<DamageFloaterManager> _floaters = new List<DamageFloaterManager>();
    static private Transform _rootCanvas;

    static Service()
    {
        CreateRoot();
    }
    public static bool IsMissingRoot()
    {
        if (_rootCanvas == null)
            return true;
        return false;
    }

    public static void CreateRoot()
    {
        // if current scene name is "Combat Map Editor" return
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat Map Editor") return;
        var prefabObj = Resources.Load($"Prefab/Canvas/WorldTextCanvas") as GameObject;
        var worldCanvasObj = GameObject.Instantiate(prefabObj, Vector3.zero, Quaternion.identity);
        _rootCanvas = worldCanvasObj.transform;
        _rootCanvas.name = "WorldTextCanvas";
        GameObject.DontDestroyOnLoad(_rootCanvas);

        _floaters.Clear();
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
        UIManager.instance.onHealed.AddListener(DamagedText); // ���ظ� �Դ� �Ͱ� ���� ��Ģ ����.
    }


    public static void MissedText(IDamageable unit)
    {
        var pos = Hex.Hex2World(unit.GetHex());
        
        Service.SetText(Damage.Type.DEFAULT, "MISS", pos, 1.0f);
    }

    public static void DamagedText(IDamageable unit, int damage, Damage.Type type)
    {
        var pos = Hex.Hex2World(unit.GetHex());
        
        // �������� ����Ͽ� Ŀ���� ���� ������ ���� ��Ȳ�� ���� "ū ������"�� ������ �ٲ�Ƿ�, ������ �ӽ÷θ�
        // 15���� ���� ������ scale�� �߰��� �ְ���
        float scale = 1.0f;
        if (damage > 15) scale = 1.5f;
        if (type.HasFlag(Damage.Type.MISS)) return;

        SetText(type, damage.ToString(), pos, scale);
    }

    /// <summary>
    /// Type�� ���� � damageFloater�� ����ϴ����� �������� List ���� ������ �̸� Ȯ��
    /// </summary>
    public static void SetText(Damage.Type type, string text, Vector3 position, float scale = 1.0f)
    {
        DamageFloaterWrapper floater = null;
        if (type.HasFlag(Damage.Type.CRITICAL))
        {
            floater = _floaters[1].Set();
        }
        else if (type.HasFlag(Damage.Type.BURNED))
        {
            floater = _floaters[0].Set();
        }
        else if (type.HasFlag(Damage.Type.BLOODED))
        {
            floater = _floaters[0].Set();
        }
        else if (type.HasFlag(Damage.Type.HEAL))
        {
            floater = _floaters[2].Set();
        }
        else
        {
            floater = _floaters[0].Set();
        }
        var randValue = new Vector3(Random.value - 0.5f, Random.value, Random.value) * 2.0f;
      
        floater.tmp.text = text;
        floater.Instance.position = position + randValue;
        floater.Instance.localScale = Vector3.one * scale * DamageFloaterWrapper.SCALE_START;
    }

    public static void OnUpdated(float deltaTime)
    {
        foreach (var ins in _floaters)
        {
            ins.OnUpdated(deltaTime);
        }
    }
}
