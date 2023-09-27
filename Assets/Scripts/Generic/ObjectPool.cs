using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolWrapper<T> where T : Transform
{
    public T Instance;
    public float LifeTime;
    public float Duration;
    public bool Enable; // true 일시 사용 가능함(Object Pool에 들어가있으며, 인게임에서 사용중이 아님)

    public ObjectPoolWrapper(T instance, float lifeTime)
    {
        Instance = instance;
        instance.gameObject.SetActive(false);
        LifeTime = lifeTime;
        Duration = 0.0f;
        Enable = true;
    }

    public virtual void Reset()
    {
        Instance.gameObject.SetActive(false);
        Duration = 0.0f;
        Enable = true;
    }
}

/// <summary>
/// 생성과 삭제가 잦아 재사용이 필요한 오브젝트에 사용하는 풀링용 클래스입니다.
/// 자식 클래스를 생성하여 Set(), OnUpdated() 를 변경해서 사용하는 것을 권장합니다.
/// </summary>
/// <typeparam name="T"> Instance를 가질 수 있는 Transform / RectTrasnform 를 취급합니다. </typeparam>
public abstract class ObjectPool<T, Q> 
    where T : Transform
    where Q : ObjectPoolWrapper<T>
{
    protected GameObject _prefab;
    protected uint _limitSize;
    protected float _generalLifeTime;
    protected Transform _root;
    protected Queue<Q> _pool;
    protected List<Q> _working;

    /// <summary>
    /// Prefab 을 Resources에서 불러와 저장, parent 설정 등 초기 1회만 실행하는 내용입니다.
    /// </summary>
    /// <param name="objectKey"> Resources.Load로 호출할 수 있는 Prefab 경로 </param>
    /// <param name="parent"> 모든 instance의 부모인 root의 부모로 배치할 Transform </param>
    /// <param name="generalLifeTime"></param>
    /// <param name="expectedSize"></param>
    /// <param name="rootName"></param>
    public virtual void Init(string objectKey, Transform parent, float generalLifeTime, uint expectedSize=10, string rootName="")
    {
        _pool = new Queue<Q>();
        _working = new List<Q>();

        _generalLifeTime = generalLifeTime;
        _limitSize = expectedSize / 2 + 1;
        _limitSize = (_limitSize <= 1) ? 2 : _limitSize;
        var root = new GameObject();
        root.transform.parent = parent;
        root.name = (string.IsNullOrEmpty(rootName)) ? objectKey : rootName;
        _root = root.transform;
        _prefab = Resources.Load(objectKey) as GameObject;
        if (_prefab == null)
            Debug.LogError($"Resources: '{objectKey}' is not exist");
    }
    
    /// <summary>
    /// Pool에서 빼내어 Object를 사용하기 시작합니다.
    /// </summary>
    /// <returns> ObjectPool에서 사용하기 시작한 Instance를 반환합니다. </returns>
    public abstract Q Set();
    /*
    {
        if (_limitSize > _pool.Count)
        {
            for (int i = 0; i < _limitSize; i++)
            {
                var ins = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _root) as T;
                var insWrap = new ObjectPoolWrapper<T>(ins, _generalLifeTime);
                _pool.Enqueue(insWrap);
            }
        }

        var target = _pool.Dequeue();
        _working.Add(target);
        target.Instance.gameObject.SetActive(true);
        return target.Instance;
    }
    */

    /// <summary>
    /// 관리하는 Scene에서 매 프레임마다 OnUpdated를 실행하여 작동합니다.
    /// </summary>
    /// <param name="deltaTime"> 클라이언트 delta Time </param>
    public virtual void OnUpdated(float deltaTime)
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
