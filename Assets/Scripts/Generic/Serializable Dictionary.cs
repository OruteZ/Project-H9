using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    /// <summary>
    /// A dictionary that can be serialized and deserialized by Unity.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Indicates whether the dictionary is constant.
        /// </summary>
        [SerializeField] private bool isConst;
        [SerializeField] private TKey defaultKey;

        /// <summary>
        /// The list of keys in the dictionary.
        /// </summary>
        [SerializeField]
        private List<TKey> keys = new ();

        /// <summary>
        /// The list of values in the dictionary.
        /// </summary>
        [SerializeField]
        private List<TValue> values = new ();

        /// <summary>
        /// Called by Unity immediately before the object's data is serialized.
        /// Converts the dictionary into two lists for serialization.
        /// </summary>
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

        /// <summary>
        /// Called by Unity immediately after the object's data is deserialized.
        /// Converts the two lists back into a dictionary.
        /// </summary>
        public void OnAfterDeserialize()
        { 
            if (keys.Count != values.Count && isConst){
                throw new Exception("there are " + keys.Count + " keys and " + values.Count +
                                    " values after deserialization." +
                                    " Make sure that both key and value types are serializable.");
            }

            Clear();

           
            while (keys.Count > values.Count)
            {
                values.Add(default);
            }

            while (keys.Count < values.Count)
            {
                keys.Add(defaultKey);
            }

            for (int i = 0; i < keys.Count; i++) Add(keys[i], values[i]);
        }
    }
}