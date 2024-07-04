using UnityEngine;
using TMPro;

public class DamageFloaterWrapper : ObjectPoolWrapper<RectTransform>
{
    public readonly TextMeshProUGUI tmp;
    public Vector3 startWorldPosition;
    public const float FLOATING_SPEED = 2f;
    public const float SCALE_START = 0.04f;
    public const float SCALE_END = 0.01f;
    public const float SCALE_SPEED = 8.0f;

    public DamageFloaterWrapper(RectTransform instance, float lifeTime, TextMeshProUGUI tmp) : base(instance, lifeTime)
    {
        this.tmp = tmp;
    }

    public override void Reset()
    {
        base.Reset();
    }
}

public class DamageFloaterManager : ObjectPool<RectTransform, DamageFloaterWrapper>
{
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize=10, string rootName="")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);

        SupplyPool(expectedSize / 2);
    }

    public override DamageFloaterWrapper Set()
    {
        if (_limitSize > _pool.Count || 0 == _pool.Count)
            SupplyPool(_limitSize);

        DamageFloaterWrapper target = _pool.Dequeue();
        target.Enable = false;
        target.Instance.gameObject.SetActive(true);
        _working.Add(target);
        return target;
    }

    private void SupplyPool(uint size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject           ins = Object.Instantiate(
                                _prefab, 
                                Vector3.zero,
                                _prefab.transform.rotation,
                                _root
                                );
            
            RectTransform        insRect = ins.GetComponent<RectTransform>();
            TextMeshProUGUI      insTmp = ins.GetComponent<TextMeshProUGUI>();
            DamageFloaterWrapper insWrap = new (insRect, _generalLifeTime, insTmp);
            _pool.Enqueue(insWrap);
        }
    }

    public override void OnUpdated(float deltaTime)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            DamageFloaterWrapper target = _working[i];
            if (target.Enable)
                continue;

            target.Instance.anchoredPosition += Vector2.up * (DamageFloaterWrapper.FLOATING_SPEED * deltaTime);
            float lerp = Mathf.Lerp(target.Instance.localScale.x, DamageFloaterWrapper.SCALE_END, deltaTime * DamageFloaterWrapper.SCALE_SPEED);
            target.Instance.localScale = Vector3.one * lerp;

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
