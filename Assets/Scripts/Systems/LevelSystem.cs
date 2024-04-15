public static class LevelSystem
{
    private static int EXP;

    public static void ReservationExp(int exp)
    {
        EXP = exp;
        UIManager.instance.combatUI.combatResultUI.SetExpInformation(exp);
        FieldSystem.onStageStart.AddListener(GetExp);
    }

    private static void GetExp()
    {
        GameManager.instance.GetExp(EXP);
        FieldSystem.onStageStart.RemoveListener(GetExp);
        EXP = 0;
    }

    /// <summary>
    /// Reservation 동작과는 상관없는, {exp} 만큼의 경험치를 즉시 얻는다.
    /// </summary>
    public static void GetExpImmediately(int exp)
    {
           GameManager.instance.GetExp(exp);
    }
}