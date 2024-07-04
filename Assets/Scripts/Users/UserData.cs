using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class UserData 
{
    private string _fileFullName; // ex) save1_1.json 
    private string _fileName; // save1_1.json 중 'save1'
    private int _version = 102;
    public bool isFirstOpen = true;

    public int Level = 1;
    public int EXP = 0;
    public Vector3Int Position;
    public UnitStat Stat;
    public HashSet<int> ClearedQuests = new HashSet<int>();
    public Dictionary<int, QuestSaveWrapper> QuestProgress = new Dictionary<int, QuestSaveWrapper>();

    public int skillPoint = 0;
    public List<int> learnedSkills = new();

    public int money = 0;
    public int equippedItemIndex = 0;
    public List<ItemSaveWrapper> inventory = new();
    public List<ItemSaveWrapper> consumableInventory = new();
    public List<ItemSaveWrapper> otherInventory = new();

    public OptionSetting optionSetting;

    public string Description = string.Empty;
    public DateTime SaveTime;
    private int _branched = 1;

    public Dictionary<string, int> Events = new Dictionary<string, int>();

    [JsonIgnore] public string FileName => _fileName;
    [JsonIgnore] public string FileFullName => _fileFullName;
    [JsonIgnore] public int Version => _version;
    [JsonIgnore] public int Branched { get { return _branched; } set { _branched = value; _fileFullName = $"{_fileName}_{_branched}.json"; } }

    public UserData(string fileName)
    {
        _fileName = fileName;
        _fileFullName = $"{fileName}_{_branched}.json";
    }
}

public static class UserDataFileSystem
{
    private static string _defaultPath = Application.persistentDataPath;
    public static string DefaultPath => _defaultPath;

    public static void New(out UserData userData)
    {
        int ind = 1;
        while (File.Exists($"{_defaultPath}/save{ind}_1.json"))
        {
            ind++;
        }

        userData = new UserData($"save{ind}");

        // load resource Assets/Resources/Map Data/World Obj Data.asset
        userData.Position = Resources.Load<WorldData>("Map Data/World Obj Data").playerPosition;
        userData.Stat = null;
        Debug.Log($"New file: save{ind}.json");
    }

    public static void Save(in UserData userData, bool isNewBranch=false)
    {
        string jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);
        var path = $"{_defaultPath}/{userData.FileFullName}";
        File.WriteAllText(path, jsonData);

        if (isNewBranch)
        {
            int branchCount = 1;
            while (File.Exists($"{_defaultPath}/{userData.FileName}_{branchCount}.json"))
            {
                branchCount++;
            }
            userData.Branched = branchCount;
        }
    }

    public static void AutoSave(in UserData userData)
    {
        string jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);
        var path = $"{_defaultPath}/autosaved.json";
        File.WriteAllText(path, jsonData);
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