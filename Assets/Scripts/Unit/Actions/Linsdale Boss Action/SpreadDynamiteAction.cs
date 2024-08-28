using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SpreadDynamiteAction : BaseAction
{
    [SerializeField] private int dynamiteCount = 3;
    [SerializeField] private int durationTurn = 3;

    private List<Vector3Int> _targetPos;
    
    public override ActionType GetActionType()
    {
        return ActionType.SpreadDynamite;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
    }

    public override bool CanExecute()
    {
        return CanExecute(Hex.zero);
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        return true;
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(DYNAMITE);
        
        //rotation of z and x set 0
        var euler = unit.transform.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        unit.transform.eulerAngles = euler;
        
        yield return new WaitForSeconds(1.5f);
    }

    /// <summary>
    /// 폭탄을 설치 할 수 있는 타일을 선택합니다.
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private static List<Tile> SelectRandomTile(IList<Tile> tiles, int count)
    {
        int size = tiles.Count;
        
        // get random index counts
        List<int> randomIndex = new ();
        for (int i = 0; i < count; i++)
        {
            int index; 
            do {
                index = Random.Range(0, size);
            } while (randomIndex.Contains(index) || DynamiteSettable(tiles[index]) is false);
            
            randomIndex.Add(index);
        }
        
        return randomIndex.Select(index => tiles[index]).ToList();
    }
    
    private static bool DynamiteSettable(Tile tile)
    {
        Vector3Int pos = tile.hexPosition;
        
        if (FieldSystem.tileSystem.GetTile(pos) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }
        
        if (tile.walkable is false)
        {
            Debug.LogWarning("Tile is not walkable");
            return false;
        }
        
        return true;
    }
    
    #region THROW

    [SerializeField]
    private GameObject dynamitePrefab;

    private void Throw() 
    {
        
        IEnumerable<Tile> tiles = FieldSystem.tileSystem.GetTilesInRange(unit.hexPosition, radius);
        List<Tile> dynamiteTiles = SelectRandomTile(tiles.ToList(), dynamiteCount);
        
        foreach (Tile tile in dynamiteTiles)
        {
            DynamiteVisualEffect visual = Instantiate(dynamitePrefab, unit.transform.position, Quaternion.identity)
                .GetComponent<DynamiteVisualEffect>();
            Dynamite dynamite = visual.GetComponent<Dynamite>();
            
            visual.SetDestination(
                tile.hexPosition, 
                () => dynamite.SetUp(unit, tile.hexPosition, durationTurn, radius, damage),
                false
                );
            
        }
    }

    public override void TossAnimationEvent(string args)
    {
        if (args != AnimationEventNames.THROW) return;
        {
            Throw();
        }
    }
    #endregion
}