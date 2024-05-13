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

    public Vector3Int Position;
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
        userData.Position = new Vector3Int(29, 18, -47);
        Save(in userData);
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
            Debug.LogError($"파일이 존재하지 않습니다: \"{filePath}\"");
            return false;
        }

        string jsonData = File.ReadAllText(filePath);
        userData = JsonConvert.DeserializeObject<UserData>(jsonData);
        return true;
    }
}