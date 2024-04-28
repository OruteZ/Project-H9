using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownIcontWrapper : ObjectPoolWrapper<RectTransform>
{
    public Tile tile { get; private set; }
    public Town.BuildingType buildingType { get; private set; }
    public TownIcontWrapper(RectTransform instance, float lifeTime) : base(instance, lifeTime)
    {
        tile = null;
        buildingType = Town.BuildingType.NULL;
    }
    public void Init(Tile tile, Town.BuildingType type) 
    {
        this.tile = tile;
        buildingType = type;
        switch (type)
        {
            case Town.BuildingType.Ammunition:
                {
                    Instance.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo("Town_Ammunition");
                    break;
                }
            case Town.BuildingType.Saloon:
                {
                    Instance.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo("Town_Saloon");
                    break;
                }
            case Town.BuildingType.Sheriff:
                {
                    Instance.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo("Town_Sheriff");
                    break;
                }
        }
        Instance.GetComponent<Image>().enabled = false;
    }
}
public class TownIconPool : ObjectPool<RectTransform, TownIcontWrapper>
{
    int count = 0;
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize = 10, string rootName = "")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);
        GameObject.Destroy(_root.gameObject);
        _root = parent;

        SupplyPool(expectedSize);
    }

    public override TownIcontWrapper Set()
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
            ins.GetComponent<RectTransform>().localPosition = Vector3.zero;
            var insRect = ins.GetComponent<RectTransform>();
            var insWrap = new TownIcontWrapper(insRect, _generalLifeTime);
            _pool.Enqueue(insWrap);
        }
    }

    public void Update()
    {
        for (int i = 0; i <_working.Count; i++)
        {
            var target = _working[i];
            if (!target.tile.inSight) continue;
            target.Instance.GetComponent<Image>().enabled = true;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.tile.transform.position);
            target.Instance.GetComponent<RectTransform>().position = screenPos;
        }
    }
}