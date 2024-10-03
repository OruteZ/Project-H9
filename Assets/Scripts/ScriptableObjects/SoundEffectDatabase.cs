using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectDatabase", menuName = "ScriptableObjects/SoundEffectDatabase", order = 1)]
public class SoundEffectDatabase : ScriptableObject
{
    public List<SFXInfo> SFXInfos;

    public AudioClip GetSFX(string name) 
    {
        foreach (var info in SFXInfos) 
        {
            if (info.name == name) 
            {
                return info.clip;
            }
        }

        Debug.LogError("Can't Find Audio Clip Name: " + name);
        return null;
    }
}

[Serializable]
public struct SFXInfo
{
    public string name;
    public AudioClip clip;
}