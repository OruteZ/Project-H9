using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillKeywordWrapper : ObjectPoolWrapper<RectTransform>
{
    public SkillKeywordTooltip tooltip;

    public SkillKeywordWrapper(RectTransform instance, float lifeTime, SkillKeywordTooltip t) : base(instance, lifeTime)
    {
        tooltip = t;
    }

    public override void Reset()
    {
        tooltip.CloseUI();
        base.Reset();
    }
}

public class SkillKeywordPool : ObjectPool<RectTransform, SkillKeywordWrapper>
{
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize = 20, string rootName = "")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);
        _root.transform.localPosition = Vector3.zero;
        _root.transform.localScale = Vector3.one;

        SupplyPool(expectedSize);
        _root.transform.gameObject.AddComponent<VerticalLayoutGroup>();
    }

    public override SkillKeywordWrapper Set()
    {
        var target = _pool.Dequeue();
        target.Enable = false;
        target.Instance.transform.SetAsLastSibling();
        target.Instance.gameObject.SetActive(true);
        _working.Add(target);
        return target;
    }

    private void SupplyPool(uint size)
    {
        for (int i = 0; i < size; i++)
        {
            var ins = GameObject.Instantiate(_prefab, Vector3.zero, _prefab.transform.rotation) as GameObject;
            ins.transform.SetParent(_root, false);
            ins.GetComponent<RectTransform>().localPosition = Vector3.zero;
            var insRect = ins.GetComponent<RectTransform>();
            var insKtt = ins.GetComponent<SkillKeywordTooltip>();
            var insWrap = new SkillKeywordWrapper(insRect, _generalLifeTime, insKtt);
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
