using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AIModel", menuName = "AI/AdenAIModel", order = 1)]
public class AdenAIModel : AIModel
{
    public int spreadDynamiteCooldown;
    public int mainDynamiteCooldown;
    public int suicideHpThreshold;

    [SerializeField, Space(10)] 
    private int curSDCooldown;
    [SerializeField]
    private int curMDCooldown;
    
    
    public override AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // �ĺ� Actions
        ReloadAction reloadAction = unit.GetAction<ReloadAction>();
        MoveAction moveAction = unit.GetAction<MoveAction>();
        AttackAction attackAction = unit.GetAction<AttackAction>();
        SpreadDynamiteAction sdAction = unit.GetAction<SpreadDynamiteAction>();
        DynamiteAction dAction = unit.GetAction<DynamiteAction>();
        SuicideDynamiteAction suicideAction = unit.GetAction<SuicideDynamiteAction>();
        
        // nullcheck
        if (reloadAction == null || 
            moveAction == null || 
            attackAction == null || 
            sdAction == null || 
            dAction == null || 
            suicideAction == null
            )
        {
            Debug.LogError("Action is null");
            return new AIResult(null, unit.hexPosition);
        }
        
        // ===============0. ���� ����================
        // 0 - 1. ����
        if (unit.stat.curHp <= suicideHpThreshold && suicideAction.CanExecute())
        {
            return new AIResult(suicideAction, unit.hexPosition);
        }
        
        // 0 - 2. �������� ���̳ʸ���Ʈ
        if (curSDCooldown <= 0 && sdAction.CanExecute())
        {
            curSDCooldown = spreadDynamiteCooldown;
            return new AIResult(sdAction, unit.hexPosition);
        }
        
        // =============== 1. Reload ===============
        if (IsOutOfAmmo(ai) && reloadAction.CanExecute())
        {
            // returns reload action
            return new AIResult(reloadAction, unit.hexPosition);
        }
        
        // =============== 2. Out Of Sight : Move to Last Position ===============
        if (IsEnemyOutOfSight(ai))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // =============== 3. In Sight ================
        // 3 - 0. �ϴ� ���� ���̸� �ִ� ��Ÿ��� ���̳ʸ���Ʈ �߻�
        // a. Throwing range check
        int throwingRange = dAction.GetThrowingRange();
        if (Hex.Distance(unit.hexPosition, ai.playerPosMemory) <= throwingRange)
        {
            // b. Cooldown check
            if (curMDCooldown <= 0 && dAction.CanExecute(ai.playerPosMemory))
            {
                curMDCooldown = mainDynamiteCooldown;
                return new AIResult(dAction, ai.playerPosMemory);
            }
        }
        
        // b. range �ٱ��̶��, Lerp �ؼ� �߰��� ���̳ʸ���Ʈ �߻�
        Vector3Int lerpPos = Hex.Lerp(unit.hexPosition, ai.playerPosMemory, throwingRange);
        if (curMDCooldown <= 0 && dAction.CanExecute(lerpPos))
        {
            curMDCooldown = mainDynamiteCooldown;
            return new AIResult(dAction, lerpPos);
        }
        
        
        // 3 - 1. ����ϱ⿡ �ʹ� �ָ� ���� ���. ������ �̵�
        if (unit.weapon.GetRange() < Hex.Distance(unit.hexPosition, ai.playerPosMemory))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // 3 - 2. ��� ������ ������ ���, ��� ���� ���� Cost�� ���� ������ ������ �̵�
        if (ai.MoveCount > 0 && Hex.Distance(unit.hexPosition, ai.playerPosMemory) > 1)
        {
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        return new AIResult(attackAction, ai.playerPosMemory);
    }
        
    public override void Setup(EnemyAI ai)
    {
        ai.GetUnit().onTurnEnd.AddListener(OnTurnEnd);
    }

    private void OnTurnEnd(Unit unit)
    {
        // ��� cooldown 1 ��
        curSDCooldown = Mathf.Max(0, curSDCooldown - 1);
        curMDCooldown = Mathf.Max(0, curMDCooldown - 1);
        
        // ����
        SuicideDynamiteAction suicideAction = unit.GetAction<SuicideDynamiteAction>();
        suicideHpThreshold = suicideAction.ExecutableHp;
    }
}