using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemListWrapper : ObjectPoolWrapper<RectTransform>
{
    public ItemListWrapper(RectTransform instance, float lifeTime) : base(instance, lifeTime)
    {
    }
    public override void Reset()
    {
        Instance.GetComponent<ItemListUIElement>().CloseUI();
        base.Reset();
    }
}
public class ItemListPool : ObjectPool<RectTransform, ItemListWrapper>
{
    int count = 0;
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize = 10, string rootName = "")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);
        GameObject.Destroy(_root.gameObject);
        _root = parent;

        SupplyPool(expectedSize);
    }

    public override ItemListWrapper Set()
    {
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
            //ins.GetComponent<RectTransform>().localPosition = Vector3.zero;
            //ins.GetComponent<RectTransform>().localPosition = new Vector3(5, -35, 0);
            var insRect = ins.GetComponent<RectTransform>();
            var insWrap = new ItemListWrapper(insRect, _generalLifeTime);
            _pool.Enqueue(insWrap);
        }
    }
    public void Reset()
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            var target = _working[i];
            if (!target.Enable)
            {
                target.Reset();
                _pool.Enqueue(target);
                _working.RemoveAt(i);
            }
        }
    }
}