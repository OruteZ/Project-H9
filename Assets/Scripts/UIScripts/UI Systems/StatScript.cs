using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScriptInfo
{
    public int index { get; private set; }
    public string originalName { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }

    public StatScriptInfo(int idx, string o, string n, string d) 
    {
        index = idx;
        originalName = o;
        name = n;
        description = d.Trim('\"').Replace("[", "<color=#FEB100>").Replace("]", "</color>");
    }
}
public class StatScript
{
    public ScriptLanguage scriptLanguage = UIManager.instance.scriptLanguage;
    private List<StatScriptInfo> _statScripts;

    public StatScript() 
    {
        _statScripts = new List<StatScriptInfo>();
        List<List<string>> statNameTable = FileRead.Read("StatNameLocalizationTable", out var nameColumnInfo);
        List<List<string>> statDescTable = FileRead.Read("StatTooltipLocalizationTable", out var descColumnInfo);
        if (statNameTable == null || statDescTable == null) 
        {
            Debug.LogError("스탯 스크립트 테이블을 찾을 수 없습니다.");
            return;
        }

        int length = statNameTable.Count;
        if (length < statDescTable.Count) length = statDescTable.Count;

        for (int i = 0; i < length; i++) 
        {
            string originName = "";
            string statName = "";
            if (i < statNameTable.Count) 
            {
                originName = statNameTable[i][(int)ScriptLanguage.English];
                statName = statNameTable[i][(int)scriptLanguage];
            }
            string statDesc = "";
            if (i < statDescTable.Count)
            {
                statDesc = statDescTable[i][(int)scriptLanguage];
            }

            StatScriptInfo info = new StatScriptInfo(i, originName, statName, statDesc);
            _statScripts.Add(info);
        }
    }

    public StatScriptInfo GetStatScript(int idx)
    {
        if (_statScripts is null)
        {
            Debug.LogError("스탯 스크립트가 존재하지 않습니다.");
            return null;
        }
        if (_statScripts.Count < idx)
        {
            Debug.LogError("스탯 스크립트에 존재하지 않는 인덱스입니다. index: " + idx);
            return null;
        }

        return _statScripts[idx];
    }
    public StatScriptInfo GetStatScript(string originStr)
    {
        if (_statScripts is null)
        {
            Debug.LogError("스탯 스크립트가 존재하지 않습니다.");
            return null;
        }

        for (int i = 0; i < _statScripts.Count; i++) 
        {
            if (_statScripts[i].originalName == originStr) 
            {
                return _statScripts[i];
            }
        }

        Debug.LogError("스탯 스크립트에 존재하지 않는 스탯입니다. stat name: " + originStr);
        return null;
    }
}
