public static class LazyLevelHandler 
{
    private static int _exp;

    public static void ReservationExp(int exp)
    {
        _exp = exp;
        UIManager.instance.combatUI.combatResultUI.SetExpInformation(exp);
        FieldSystem.onStageStart.AddListener(GetExp);
    }

    private static void GetExp()
    {
        GameManager.instance.LevelSystem.GetExp(_exp);
        FieldSystem.onStageStart.RemoveListener(GetExp);
        _exp = 0;
    }
}
