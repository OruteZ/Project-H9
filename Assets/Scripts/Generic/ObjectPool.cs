using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private class Wrapper
    {
        public Wrapper(GameObject instance, float lifeTime)
        {
            Instance = instance;
            instance.SetActive(false);
            LifeTime = lifeTime;
            Duration = 0.0f;
            Enable = true;
        }

        public void Reset()
        {
            Instance.SetActive(false);
            Duration = 0.0f;
            Enable = true;
        }

        public GameObject Instance;
        public float LifeTime;
        public float Duration;
        public bool Enable; // true 일시 사용 가능함(Object Pool에 들어가있으며, 인게임에서 사용중이 아님)
    }

    private GameObject _prefab;
    private uint _limitSize;
    private float _generalLifeTime;
    private Transform _root;
    private Queue<Wrapper> _pool;
    private List<Wrapper> _working;

    public void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize=10, string rootName="")
    {
        _pool = new Queue<Wrapper>();
        _working = new List<Wrapper>();

        _generalLifeTime = generalLifeTime;
        _limitSize = expectedSize / 2 + 1;
        var root = new GameObject();
        root.transform.parent = parent;
        root.name = (string.IsNullOrEmpty(rootName)) ? objectKey : rootName;
        _root = root.transform;
        _prefab = Resources.Load(objectKey) as GameObject;
        if (_prefab == null)
            Debug.LogError($"Resources: '{objectKey}' is not exist");
    }

    public GameObject Set(Vector3 position)
    {
        if (_limitSize > _pool.Count)
        {
            for (int i = 0; i < _limitSize; i++)
            {
                var ins = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _root);
                var insWrap = new Wrapper(ins, _generalLifeTime);
                _pool.Enqueue(insWrap);
            }
        }

        var target = _pool.Dequeue();
        _working.Add(target);
        target.Instance.transform.position = position;
        target.Instance.SetActive(true);
        return target.Instance;
    }
    
    /// <summary>
    /// 관리하는 Scene에서 하나의 OnUpdated를 실행해주어야 작동합니다.
    /// </summary>
    public void OnUpdated(float deltaTime)
    {
        for (int i = _working.Count - 1; 0 <= i; i--)
        {
            var target = _working[i];
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
