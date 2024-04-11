using System;
using System.Collections.Generic;
using UnityEngine;
using QUEST_EVENT = QuestInfo.QUEST_EVENT;

public class QuestInit
{
    private readonly string QUEST_FILE_PATH = $"QuestTable";
    private readonly string QUEST_LOCALIZATION_PATH = $"QuestLocalizationTable";
    public List<QuestInfo> GetQuests()
    {
        ParseLocalization(out var localizationData);
        ParseQuestInfos(in localizationData, out var questInfos);
        // !유저데이터와 동기화하는 함수() 필요
        return questInfos;
    }

    // private --
    // ReadFile에 때려박아서 Localization Table은 무조건 이 코드를 거치게 만들까
    // 사용자 언어에 따라 [인덱스, 언어1, 언어2, 언어3 ... ]을 [인덱스, 사용자 언어] 로 반환하는 코드
    private bool ParseLocalization(out Dictionary<int, string> localizationData)
    {
        localizationData = null;
        var file = FileRead.Read(QUEST_LOCALIZATION_PATH, out var columnInfo);
        if (file is null)
        {
            Debug.LogError("There is no QuestLocalizationTable");
            return false;
        }

        int languageIndex = 0;
        switch (UserAccount.Language)
        {
            case ScriptLanguage.Korean:
                languageIndex = columnInfo["kor"];
                break;
            case ScriptLanguage.English:
                languageIndex = columnInfo["eng"];
                break;
            case ScriptLanguage.NULL:
                Debug.LogError("Quest Localization is doing Before Set UserAccount Language.");
                break;
        };

        localizationData = new Dictionary<int, string>();
        for (int i = 0; i < file.Count; i++)
        {
            var line = file[i];

            try
            {
                int index = int.Parse(line[0]);
                string item = line[languageIndex];
                localizationData.Add(index, item);
            }
            catch
            {
                string lineSum = "";
                for (int j = 0; j < line.Count; j++)
                    lineSum += $"[{j}] {line[j]}\n";
                Debug.LogError($"QuestLocalization error: ({i} line) {lineSum}");
                break;
            }
        }

        return true;
    }

    private bool ParseQuestInfos(in Dictionary<int, string> localizationData, out List<QuestInfo> questInfos)
    {
        questInfos = null;
        var file = FileRead.Read(QUEST_FILE_PATH, out var columnInfo);
        if (file is null)
        {
            Debug.LogError($"There is no QuestTable");
            return false;
        }

        questInfos = new List<QuestInfo>();
        for (int i = 0; i < file.Count; i++)
        {
            var line = file[i];

            int index = int.Parse(line[0]);
            int questType = int.Parse(line[1]);
            int questNameRef = int.Parse(line[2]);
            string questName = localizationData[questNameRef];
            int questTooltipRef = int.Parse(line[3]);
            string questTooltip = localizationData[questTooltipRef];
            int startScript = int.Parse(line[4]);
            int endScript = int.Parse(line[5]);

            QUEST_EVENT conditionBit = ParseQuestEvent(line[6]);
            string[] sGetConditionArguments = line[7].Replace("\"", "").Split(",");
            int[] getConditionArguments = Array.ConvertAll(sGetConditionArguments, e => int.Parse(e));
            int expireTurn = int.Parse(line[8]);
            QUEST_EVENT goalBit = ParseQuestEvent(line[9]);
            string[] sgoalArguemnts = line[10].Replace("\"", "").Split(",");
            int[] goalArguemnts = Array.ConvertAll(sgoalArguemnts, e => int.Parse(e));

            int moneyReward = int.Parse(line[11]);
            int expReward = int.Parse(line[12]);
            int itemReward = int.Parse(line[13]);
            int skillReward = int.Parse(line[14]);

            questInfos.Add(new QuestInfo(index
                                        , questType
                                        , questName
                                        , questTooltip
                                        , startScript
                                        , endScript
                                        , conditionBit
                                        , getConditionArguments
                                        , expireTurn
                                        , goalBit
                                        , goalArguemnts
                                        , moneyReward
                                        , expReward
                                        , itemReward
                                        , skillReward));
        }
        return true;
    }

    private QUEST_EVENT ParseQuestEvent(string str)
    {
        QUEST_EVENT bitEvent = QUEST_EVENT.NULL;
        if (str.Contains("GAME_START")) bitEvent |= QUEST_EVENT.GAME_START;
        if (str.Contains("MOVE_TO")) bitEvent |= QUEST_EVENT.MOVE_TO;
        if (str.Contains("QUEST_END")) bitEvent |= QUEST_EVENT.QUEST_END;
        if (str.Contains("CONVERSATION")) bitEvent |= QUEST_EVENT.SCRIPT;
        if (str.Contains("GET_ITEM")) bitEvent |= QUEST_EVENT.GET_ITEM;
        if (str.Contains("USE_ITEM")) bitEvent |= QUEST_EVENT.USE_ITEM;
        if (str.Contains("KILL_LINK")) bitEvent |= QUEST_EVENT.KILL_LINK;
        if (str.Contains("KILL_TARGET")) bitEvent |= QUEST_EVENT.KILL_TARGET;
        return bitEvent;
    }
}