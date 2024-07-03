using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DGS = System.Diagnostics;

public class QuestCallback
{
    public QuestCallback( List<QuestInfo> Quests
                        , UserData user
                        , UnityEvent OnGameStarted
                        , UnityEvent<QuestInfo> OnNotifiedQuestEnd
                        , UnityEvent<QuestInfo> OnNotifiedQuestStart
                        , UnityAction<QuestInfo> InvokeQuestStart
                        , UnityAction<QuestInfo> InvokeQuestEnd
        )
    {
        var watch = DGS.Stopwatch.StartNew();
        foreach (var quest in Quests)
        {
            // 이미 클리어한 퀘스트라면, 표기 후 건들지 않을 예정
            if (user.ClearedQuests.Contains(quest.Index))
            {
                quest.SetClear();
                continue;
            }

            // 퀘스트 진척도에 따라 동기화함
            // "진행중인" 퀘스트가 아니더라도, "퀘스트를 진행하기 위한 조건"이 때문에 거의 모든 퀘스트가 동기화됨
            // 여기에 속하지 않는것은 "새로 생긴 퀘스트"
            if (user.QuestProgress.ContainsKey(quest.Index))
            {
                var progress = user.QuestProgress[quest.Index];
                quest.SetProgress(progress);
            }

            // 이미 진행중인 퀘스트는 UI에 표기
            if (quest.IsInProgress)
            {
                UIManager.instance.gameSystemUI.questUI.AddQuestListUI(quest);
            }

            // 퀘스트 수주 조건
            // - 이미 수주한 퀘스트에는 할당하지 않음
            if (quest.IsInProgress == false)
            {
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GAME_START))
                    OnGameStarted.AddListener(quest.OnConditionEventOccured);
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                    OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedConditionEvent(q.Index));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                    PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedConditionEvent(pos));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                    GameManager.instance.onPlayerCombatFinished.AddListener(quest.OnCountConditionEvented);
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                    FieldSystem.unitSystem.onAnyUnitDead.AddListener((u) => quest.OnCountConditionEvented(u.Index));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                    IInventory.OnGetItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                    IInventory.OnUseItem.AddListener((i) => quest.OnCountConditionEvented(i.id));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                    PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedConditionEvent(tile.hexPosition));
                if (quest.HasConditionFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                    PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedConditionEvent(link.linkIndex));
                quest.OnQuestStarted.AddListener(InvokeQuestStart);
            }

            // 퀘스트 완료 조건
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.QUEST_END))
                OnNotifiedQuestEnd.AddListener((q) => quest.OnAccordedGoalEvent(q.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.MOVE_TO))
                PlayerEvents.OnMovedPlayer.AddListener((pos) => quest.OnPositionMovedGoalEvent(pos));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_LINK))
                GameManager.instance.onPlayerCombatFinished.AddListener(quest.OnCountGoalEvented);
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.KILL_UNIT))
                FieldSystem.unitSystem.onAnyUnitDead.AddListener((u) => quest.OnCountGoalEvented(u.Index));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.GET_ITEM))
                IInventory.OnGetItem.AddListener((i) => quest.OnCountGoalEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.USE_ITEM))
                IInventory.OnUseItem.AddListener((i) => quest.OnCountGoalEvented(i.id));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.TILE_IN_SIGHT))
                PlayerEvents.OnEnteredTileinSight.AddListener((tile) => quest.OnPositionMovedGoalEvent(tile.hexPosition));
            if (quest.HasGoalFlag(QuestInfo.QUEST_EVENT.LINK_IN_SIGHT))
                PlayerEvents.OnEnteredLinkinSight.AddListener((link) => quest.OnAccordedGoalEvent(link.linkIndex));
            if (quest.ExpireTurn != -1)
                PlayerEvents.OnProcessedWorldTurn.AddListener((u) => { quest.ProgressExpireTurn(); });
            quest.OnQuestEnded.AddListener(InvokeQuestEnd);
        }
        watch.Stop();
        Debug.Log($"<color=blue>Quest link time: {watch.ElapsedMilliseconds}</color>");

        // 퀘스트 앞/뒤 대화 콜백
        OnNotifiedQuestStart.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, true); });
        OnNotifiedQuestEnd.AddListener((q) => { UIManager.instance.gameSystemUI.conversationUI.PrepareToStartConversation(q, false); });

    }
}
