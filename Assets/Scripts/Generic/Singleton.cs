using System;
using UnityEngine;

namespace Generic
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
            
                //없으면 동일 타입의 오브젝트 탐색
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance != null) return _instance;
            
                //찾아도 없으면 오브젝트 생성
                var obj = new GameObject(typeof(T).Name, typeof(T));
                _instance = obj.GetComponent<T>();

                return _instance;
            }
        }

        protected void Awake()
        {
#if UNITY_EDITOR
            if (_instance != null)
            {
                Debug.LogError("Singleton 2회 생성" + nameof(T));
                Destroy(gameObject);
            }
#endif
            if (transform.parent != null && transform.root != null)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad()
        {
            _instance = null;
        }
    }
}