public static class LazyLevelHelper 
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
}
