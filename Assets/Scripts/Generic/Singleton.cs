using System;
using UnityEngine;

namespace Generic
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T instance
        {
            get
            {
                if (_instance is not null) return _instance;
            
                //없으면 동일 타입의 오브젝트 탐색
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance is not null) return _instance;
            
                //찾아도 없으면 오브젝트 생성
                var obj = new GameObject(typeof(T).Name, typeof(T));
                _instance = obj.GetComponent<T>();

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance is not null)
            {
                if (gameObject != _instance.gameObject)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
            else 
            {
                _instance = instance;
            }

            if (transform.parent != null && transform.root != null)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected void OnDestroy()
        {
            _instance = null;
        }

        // [RuntimeInitializeOnLoadMethod]
        // private static void RuntimeInitializeOnLoad()
        // {
        //     _instance = null;
        // }
    }
}