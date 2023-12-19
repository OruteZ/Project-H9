using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private bool isConst;
        
        [SerializeField]
        private List<TKey> keys = new ();


        [SerializeField]
        private List<TValue> values = new ();


        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }


        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            
            Clear();

            if (keys.Count != values.Count && isConst){
                throw new Exception("there are " + keys.Count + " keys and " + values.Count + 
                                    " values after deserialization." +
                                        " Make sure that both key and value types are serializable.");
            }

            while (keys.Count > values.Count)
            {
                values.Add(default);
            }
            
            while (keys.Count < values.Count)
            {
                keys.Add(default);
            }

            for (int i = 0; i < keys.Count; i++) Add(keys[i], values[i]); 
        }
    }


}