using UnityEngine;
using UnityEngine.Events;

public static class PlayerEvents
{
    public static UnityEvent<Vector3Int> OnMovedPlayer = new UnityEvent<Vector3Int>();
    public static UnityEvent<Link> OnEnteredLinkinSight = new UnityEvent<Link>();
    public static UnityEvent<Tile> OnEnteredTileinSight = new UnityEvent<Tile>();
    public static UnityEvent<int> OnProcessedWorldTurn = new UnityEvent<int>();
    public static UnityEvent<SkillInfo> OnLearnedSkill = new UnityEvent<SkillInfo>();
    public static UnityEvent<int> OnGetMoney = new UnityEvent<int>();
    public static UnityEvent<Item> OnGetItem = new UnityEvent<Item>();
    public static UnityEvent<QuestInfo> OnStartedQuest = new UnityEvent<QuestInfo>();
    public static UnityEvent<QuestInfo> OnSuccessQuest = new UnityEvent<QuestInfo>();
    public static UnityEvent<QuestInfo> OnFailedQuest = new UnityEvent<QuestInfo>();
    public static UnityEvent<UnitStat, StatType> OnChangedStat = new UnityEvent<UnitStat, StatType>();
    public static UnityEvent OnIncSkillPoint = new UnityEvent();
    public static UnityEvent OnIncStatPoint = new UnityEvent();
    public static UnityEvent<Weapon> OnWeaponChanged = new UnityEvent<Weapon>();
    public static UnityEvent<Vector3Int, Vector3Int, int, Town.BuildingType> OnPlayerEnterTown = new UnityEvent<Vector3Int, Vector3Int, int, Town.BuildingType>();
}
