using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UnitSpeechWrapper : ObjectPoolWrapper<RectTransform>
{
    public UnitSpeechWrapper(RectTransform instance, float lifeTime) : base(instance, lifeTime)
    {
    }
    public override void Reset()
    {
        base.Reset();
    }
}

public class UnitSpeechPool : ObjectPool<RectTransform, UnitSpeechWrapper>
{
    int count = 0;
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize = 10, string rootName = "")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);
        GameObject.Destroy(_root.gameObject);
        _root = parent;

        SupplyPool(expectedSize);
    }

    public override UnitSpeechWrapper Set()
    {
        if (_pool.Count <= 0)
        {
            SupplyPool(10);
        }
        var target = _pool.Dequeue();

        target.Enable = false;

        _working.Add(target);
        target.Instance.gameObject.SetActive(true);
        return target;
    }

    private void SupplyPool(uint size)
    {
        for (int i = 0; i < size; i++)
        {
            var ins = GameObject.Instantiate(_prefab, Vector3.zero, _prefab.transform.rotation);
            ins.name = _prefab.name + " " + count++;
            ins.transform.SetParent(_root, false);
            ins.GetComponent<RectTransform>().localPosition = Vector3.zero;
            var insRect = ins.GetComponent<RectTransform>();
            var insWrap = new UnitSpeechWrapper(insRect, _generalLifeTime);
            _pool.Enqueue(insWrap);
        }
    }
    public void Reset(GameObject targetUI)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            if (targetUI != _working[i].Instance.gameObject) continue;
            var target = _working[i];
            target.Reset();
            _pool.Enqueue(target);
            _working.RemoveAt(i);
            return;
        }
    }
    public UnitSpeechWrapper FindByUnit(Unit unit)
    {
        for (int i = 0; i < _working.Count; i++)
        {
            if (_working[i].Instance == unit.gameObject)
            {
                return _working[i];
            }
        }
        return null;
    }
}