using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class UserData 
{
    public string FileFullName; // ex) save1_1.json 
    public string FileName; // save1_1.json 중 'save1'
    public int Version = 103;
    public bool isFirstOpen = true;
    
    public WorldData worldData;

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
    private int Branched = 1;

    public Dictionary<string, int> Events = new Dictionary<string, int>();

    public UserData(string fileName)
    {
        FileName = fileName;
        FileFullName = $"{fileName}_{Branched}.json";
    }

    public void SetBranch(int branch)
    {
        Branched = branch;
        FileFullName = $"{FileName}_{branch}.json";
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
        Debug.Log($"New File {userData.FileName}");
        // load resource Assets/Resources/Map Data/World Obj Data.asset
        userData.Position = Resources.Load<WorldData>("Map Data/World Obj Data").playerPosition;
        userData.Stat = null;
    }

    public static void Save(in UserData userData, bool isNewBranch=false)
    {
        string jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);

        if (isNewBranch)
        {
            int branchCount = 1;
            while (File.Exists($"{_defaultPath}/{userData.FileName}_{branchCount}.json"))
            {
                branchCount++;
            }
            userData.SetBranch(branchCount);
        }
        var path = $"{_defaultPath}/{userData.FileFullName}";
        Debug.Log($"saved {path}");
        File.WriteAllText(path, jsonData);
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