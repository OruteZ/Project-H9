public static class LazyLevelHandler 
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
        GameManager.instance.LevelSystem.GetExp(EXP);
        FieldSystem.onStageStart.RemoveListener(GetExp);
        EXP = 0;
    }
}
