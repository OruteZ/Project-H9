using UnityEngine;

public class CLevelSystem
{
    public CLevelSystem(int level, int curExp)
    {
        _level = level;
        _curExp = curExp;

        if (_curExp >= MaxExp)
        {
            Debug.LogWarning($"초기 curExp가 너무 높게 설정되었습니다({curExp}/{MaxExp}). curExp가 maxExp보다 높지만, 레벨업을 진행하지 않고 진행합니다. 처음 경험치를 획득할 시 레벨업이 함께 진행될 예정입니다.\n만약, 시작부터 5레벨인상태로 시작하고싶다면 원활한 이용을 위해 초기 level을 건드는 것이 아니라, 경험치를 늘리는 치트를 만드는 것을 권장합니다.");
        }
    }

    private int _level = 1;
    private int _curExp = 0;
    public int Level => _level;
    public int CurExp => _curExp;
    public int MaxExp => _level * 100;
    private const int LEVEL_UP_REWARD_SKILL_POINT = 3;
    private const int PLAYER_STAT_CONDITION_INTERVAL = 1;

    public void GetExp(int exp)
    {
        _curExp += exp;
        while (_curExp >= MaxExp)
        {
            LevelUp();
        }
        UIManager.instance.onPlayerStatChanged.Invoke();
        UIManager.instance.onGetExp.Invoke(exp);
    }
    private void LevelUp()
    {
        if (MaxExp > _curExp) return;

        _curExp -= MaxExp;
        _level++;

        var user = GameManager.instance.user;
        user.Stat.Recover(StatType.CurHp, user.Stat.GetStat(StatType.MaxHp), out int appliedValue);
        SkillManager.instance.AddSkillPoint(LEVEL_UP_REWARD_SKILL_POINT);
        if (_level % PLAYER_STAT_CONDITION_INTERVAL == 0)
        {
            UIManager.instance.gameSystemUI.playerStatLevelUpUI.AddPlayerStatPoint();
            UIManager.instance.onLevelUp.Invoke(_level);
        }
    }
}
