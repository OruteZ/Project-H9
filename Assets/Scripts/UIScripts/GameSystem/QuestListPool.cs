using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListWrapper : ObjectPoolWrapper<RectTransform>
{
    public QuestListWrapper(RectTransform instance, float lifeTime) : base(instance, lifeTime)
    {
    }
    public override void Reset()
    {
        Instance.GetComponent<QuestListElement>().CloseUI();
        base.Reset();
    }
}
public class QuestListPool : ObjectPool<RectTransform, QuestListWrapper>
{
    int count = 0;
    public override void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize = 10, string rootName = "")
    {
        base.Init(objectKey, parent, generalLifeTime, expectedSize, rootName);
        GameObject.Destroy(_root.gameObject);
        _root = parent;

        SupplyPool(expectedSize);
    }

    public override QuestListWrapper Set()
    {
        if (_pool.Count == 0) SupplyPool(10);
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
            var insWrap = new QuestListWrapper(insRect, _generalLifeTime);
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
    public QuestListElement Find(int idx)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            var target = _working[i];
            if (target.Instance.GetComponent<QuestListElement>().currentQuestInfo.Index == idx)
            {
                return target.Instance.GetComponent<QuestListElement>();
            }
        }
        Debug.LogError("해당 인덱스의 퀘스트가 활성화되어있지 않습니다. 인덱스: " + idx);
        return null;
    }
    public void Remove(int idx)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            var target = _working[i];
            if (target.Instance.GetComponent<QuestListElement>().currentQuestInfo.Index == idx)
            {
                target.Reset();
                _pool.Enqueue(target);
                _working.RemoveAt(i);
                return;
            }
        }
        Debug.LogError("해당 인덱스의 퀘스트가 활성화되어있지 않습니다. 인덱스: " + idx);
    }
    public void Sort()
    {
        List<QuestListWrapper> mainQuests = new List<QuestListWrapper>();
        List<QuestListWrapper> subQuests = new List<QuestListWrapper>();
        for (int i = 0; i < _working.Count; i++)
        {
            var target = _working[i];
            if (target.Instance.GetComponent<QuestListElement>().currentQuestInfo.QuestType == 1)
            {
                mainQuests.Add(target);
            }
            else
            {
                subQuests.Add(target);
            }
        }

        for (int i = 0; i < mainQuests.Count; i++)
        {
            var target = mainQuests[i];
            target.Instance.SetAsLastSibling();
        }
        for (int i = 0; i < subQuests.Count; i++)
        {
            var target = subQuests[i];
            target.Instance.SetAsLastSibling();
        }
    }
}
