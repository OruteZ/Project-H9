using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class UserData 
{
    private string _fileName;
    public string FileName => _fileName;
    private int _version = 102;
    public bool isFirstOpen = true;

    public Vector3Int Position;
    public UnitStat Stat;
    public HashSet<int> ClearedQuests = new HashSet<int>();
    public Dictionary<int, QuestSaveWrapper> QuestProgress = new Dictionary<int, QuestSaveWrapper>();
    public int Version => _version;

    public Dictionary<string, int> Events = new Dictionary<string, int>();

    public UserData(string fileName)
    {
        _fileName = fileName;
    }
}

public static class UserDataFileSystem
{
    private static string _defaultPath = Application.persistentDataPath;
    public static string DefaultPath => _defaultPath;

    public static void New(out UserData userData)
    {
        int ind = 1;
        while (File.Exists($"{_defaultPath}/save{ind}.json"))
        {
            ind++;
        }

        userData = new UserData($"save{ind}.json");

        // load resource Assets/Resources/Map Data/World Obj Data.asset
        userData.Position = Resources.Load<WorldData>("Map Data/World Obj Data").playerPosition;
        userData.Stat = null;
        Debug.Log($"New file: save{ind}.json");
    }

    public static void Save(in UserData userData)
    {
        string jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);
        var path = $"{_defaultPath}/{userData.FileName}";
        File.WriteAllText(path, jsonData);
        Debug.Log($"Save {userData.FileName} => {path}");
    }

    public static bool Load(out UserData userData, string filePath)
    {
        userData = null;
        if (!File.Exists(filePath))
        {
            Debug.LogError($"UserData Load failed \"{filePath}\"");
            return false;
        }

        string jsonData = File.ReadAllText(filePath);
        userData = JsonConvert.DeserializeObject<UserData>(jsonData);
        return true;
    }
}