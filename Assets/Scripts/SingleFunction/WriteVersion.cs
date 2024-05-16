using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WriteVersion : MonoBehaviour
{
    public void Awake()
    {
        var version = PlayerSettings.bundleVersion;
        GetComponent<TMPro.TMP_Text>().text = $"version {version}";
    }
}
