using UnityEngine;
using TMPro;

public class DamageFlaoterWrapper : ObjectPoolWrapper<RectTransform>
{
    public TextMeshProUGUI TMP;

    public DamageFlaoterWrapper(RectTransform instance, float lifeTime, TextMeshProUGUI tmp) : base(instance, lifeTime)
    {
        TMP = tmp;
    }
}

public class DamageFloaterManager : ObjectPool<RectTransform, DamageFlaoterWrapper>
{
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize=10, string rootName="")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);

        SupplyPool(expectedSize / 2);
    }

    public override DamageFlaoterWrapper Set()
    {
        if (_limitSize > _pool.Count || 0 == _pool.Count)
            SupplyPool(_limitSize);

        var target = _pool.Dequeue();
        target.Enable = false;
        target.Instance.gameObject.SetActive(true);
        _working.Add(target);
        return target;
    }

    private void SupplyPool(uint size)
    {
        for (int i = 0; i < size; i++)
        {
            var ins = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _root) as GameObject;
            var insRect = ins.GetComponent<RectTransform>();
            var insTmp = ins.GetComponent<TextMeshProUGUI>();
            var insWrap = new DamageFlaoterWrapper(insRect, _generalLifeTime, insTmp);
            _pool.Enqueue(insWrap);
        }
    }

    public override void OnUpdated(float deltaTime)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            var target = _working[i];
            if (target.Enable)
                continue;

            target.Duration += deltaTime;
            if (target.LifeTime <= target.Duration)
            {
                target.Reset();
                _pool.Enqueue(target);
                _working.RemoveAt(i);
            }
        }
    }
}
