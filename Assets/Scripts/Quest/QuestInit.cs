using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestInit
{
    private readonly string FILE_PATH = $"QuestTable";
    public List<QuestInfo> GetQuests(out bool isSuccess)
    {
        isSuccess = false;
        List<QuestInfo> quests = null;
        var file = FileRead.Read(FILE_PATH);
        if (file is null)
        {
            Debug.LogError("There is no LinkTable");
            return null;
        }

        for (int i = 0; i < file.Count; i++)
        {
            var line = file[i];

            try
            {
                int index = int.Parse(line[0]);
                int questType = int.Parse(line[1]);
                int questNameRef = int.Parse(line[2]);
                int questTooltipRef = int.Parse(line[3]);
                int startScript = int.Parse(line[4]);
                int endScript = int.Parse(line[5]);

                string getCondition = line[6];

                string[] sGetConditionArguments = line[7].Replace("\"", "").Split(",");
                int[] getConditionArguments = Array.ConvertAll(sGetConditionArguments, e => int.Parse(e));
                int expireTurn = int.Parse(line[8]);
                int goalType = int.Parse(line[9]);
                string[] sgoalArguemnts = line[10].Replace("\"", "").Split(",");
                int[] goalArguemnts = Array.ConvertAll(sgoalArguemnts, e => int.Parse(e));

                int moneyReward = int.Parse(line[11]);
                int expReward = int.Parse(line[12]);
                int itemReward = int.Parse(line[13]);
                int skillReward = int.Parse(line[14]);
            }
            catch
            {
                string lineSum = "";
                for (int j = 0; j < line.Count; j++)
                    lineSum += $"[{j}] {line[j]}\n";
                Debug.LogError($"QuestInfo error: ({i} line) {lineSum}");
                break;
            }
        }

        return quests;
    }
}
