public static class LevelSystem
{
    // private static LevelSystem _instance;
    //
    // private static LevelSystem instance
    // {
    //     get
    //     {
    //         if (_instance is null) _instance = new LevelSystem();
    //         return _instance;
    //     }
    // }
    //
    // private LevelSystem()
    // {
    //     if (_instance is not null) _instance = this;
    // }
    // private static bool _hasExpReservationMade;
    private static int _exp;

    public static void ReservationExp(int exp)
    {
        _exp = exp;
        UIManager.instance.combatUI.combatResultUI.SetExpInformation(exp);
        FieldSystem.onCombatStart.AddListener(GetExp);
    }

    private static void GetExp()
    {
        GameManager.instance.GetExp(_exp);
        FieldSystem.onCombatStart.RemoveListener(GetExp);
        _exp = 0;
    }
}