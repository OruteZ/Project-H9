#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WriteVersion : MonoBehaviour
{
    public string version;

    private void Awake()
    {
#if UNITY_EDITOR
        version = PlayerSettings.bundleVersion;
#endif
        GetComponent<TMPro.TMP_Text>().text = $"version {version}";
    }
}