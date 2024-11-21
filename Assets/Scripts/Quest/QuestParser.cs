using System;
using System.Collections.Generic;
using UnityEngine;
using QUEST_EVENT = QuestInfo.QUEST_EVENT;

public class QuestParser
{
    private const string QUEST_FILE_PATH = "QuestTable";
    private const string QUEST_LOCALIZATION_PATH = "QuestLocalizationTable";
    public List<QuestInfo> GetQuests()
    {
        ParseLocalization(out Dictionary<int, string> localizationData);
        ParseQuestInfos(in localizationData, out List<QuestInfo> questInfos);
        // TODO : 유저데이터와 동기화하는 함수() 필요
        return questInfos;
    }

    // private --
    private bool ParseLocalization(out Dictionary<int, string> localizationData)
    {
        if (!FileRead.ParseLocalization(QUEST_LOCALIZATION_PATH, out localizationData))
            return false;
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
            string questName = localizationData.GetValueOrDefault(questNameRef, "Null Name");
            int questTooltipRef = int.Parse(line[3]);
            string questTooltip = localizationData.GetValueOrDefault(questTooltipRef, "Null Tooltip");
            int startScript = int.Parse(line[4]);
            int endScript = int.Parse(line[5]);

            QUEST_EVENT conditionBit = ParseQuestEvent(line[6]);
            string[] sGetConditionArguments = line[7].Replace("\"", "").Split(",");
            int[] getConditionArguments = Array.ConvertAll(sGetConditionArguments, e => int.Parse(e));
            int expireTurn = int.Parse(line[8]);
            QUEST_EVENT goalBit = ParseQuestEvent(line[9]);
            string[] sgoalArguemnts = line[10].Replace("\"", "").Split(",");
            int[] goalArguemnts = Array.ConvertAll(sgoalArguemnts, e => int.Parse(e));

            string[] sPinTile = line[11].Replace("\"", "").Split(",");
            int[] pinTile = !IsEmpty(sPinTile) ? Array.ConvertAll(sPinTile, e => int.Parse(e)) : new int[] { };
            string[] sCreateLink = line[12].Replace("\"", "").Split(",");
            int[] createLink =  !IsEmpty(sCreateLink)? Array.ConvertAll(sCreateLink, e => int.Parse(e)) : new int[] { };

            int moneyReward = int.Parse(line[13]);
            int expReward = int.Parse(line[14]);
            int itemReward = int.Parse(line[15]);
            int skillReward = int.Parse(line[16]);
            string questSheriff = line[17];

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
                                        , pinTile
                                        , createLink
                                        , moneyReward
                                        , expReward
                                        , itemReward
                                        , skillReward
                                        , questSheriff));
        }
        return true;
    }

    private bool IsEmpty(string[] args)
    {
        if (args.Length == 1 && args[0] == string.Empty) return true;
        return false;
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
        if (str.Contains("KILL_UNIT")) bitEvent |= QUEST_EVENT.KILL_UNIT;
        if (str.Contains("LINK_IN_SIGHT")) bitEvent |= QUEST_EVENT.LINK_IN_SIGHT;
        if (str.Contains("TILE_IN_SIGHT")) bitEvent |= QUEST_EVENT.TILE_IN_SIGHT;
        return bitEvent;
    }
}