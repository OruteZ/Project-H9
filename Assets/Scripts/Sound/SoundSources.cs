using Generic;
using UnityEngine;

//create
[CreateAssetMenu(fileName = "SoundSources", menuName = "Sound/SoundSources")]
public class SoundSources : ScriptableObject
{
    public SerializableDictionary<string, AudioClip> soundSources = new SerializableDictionary<string, AudioClip>();
    
}