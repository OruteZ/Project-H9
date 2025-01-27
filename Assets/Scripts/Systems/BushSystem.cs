using System.Collections.Generic;
using UnityEngine;

public class BushSystem : MonoBehaviour
{
    private static BushSystem _instance;
    public static BushSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("BushSystem").AddComponent<BushSystem>();
            }

            return _instance;
        }
        
        private set => _instance = value;
    }

    /// <summary>
    /// ��� Bush�� �����ϴ� ��ųʸ�(Ȥ�� �ٸ� �ڷᱸ��)
    /// Key: HexPosition, Value: Bush
    /// </summary>
    private Dictionary<Vector3Int, Bush> _bushes = new Dictionary<Vector3Int, Bush>();
    private Dictionary<Bush, int> _bushesGroup = new Dictionary<Bush, int>();
    
    /// <summary>
    /// ������ ���� (�ν� �ȿ��� �������̰ų�, �����̳� ǥ�İ����� ó�¾Ҵٰų�)�� ���� �ʴ� ���ֵ��� ����Ʈ�Դϴ�.
    /// </summary>
    private List<Unit> _nonHideUnits = new List<Unit>();
    
    
    /// <summary>
    /// �þ� ���� ��Ұ� �ٲ𶧸��� �������� ȣ���ϱ� ���ؼ� �÷��̾� Reference ����
    /// </summary>
    Player _player;
    
    private void Awake()
    {
        Instance = this;
        
        _bushes.Clear();
        _bushesGroup.Clear();
        _nonHideUnits.Clear();
        
        FieldSystem.onStageAwake.AddListener(Group);
        FieldSystem.onStageAwake.AddListener(() => _player = FieldSystem.unitSystem.GetPlayer());
    }
    
    public void AddBush(Vector3Int position, Bush bush, bool isInitializing = false)
    {
        _bushes.Add(position, bush);
        
        // �ʱ� ���� �۾����� �߰��� bush�� �ƴ϶��, re-Grouping�� ���־�� �մϴ�.
        if (!isInitializing)
        {
            Group();
            _player.ReloadSight();
        }
    }
    
    public void RemoveBush(Vector3Int position)
    {
        _bushes.Remove(position);
        
        Group();
        _player.ReloadSight();
    }
    
    public bool IsBush(Vector3Int position)
    {
        return _bushes.ContainsKey(position);
    }
    
    private int GroupNumber(Bush bush)
    {
        return _bushesGroup[bush];
    }
    private int GroupNumber(Vector3Int position)
    {
        if (!_bushes.ContainsKey(position))
        {
            return -1;
        }
        
        return _bushesGroup[_bushes[position]];
    }
    
    
    public bool IsSameGroup(Bush a, Bush b)
    {
        return GroupNumber(a) == GroupNumber(b);
    }
    public bool IsSameGroup(Vector3Int a, Vector3Int b)
    {
        return GroupNumber(a) == GroupNumber(b);
    }
    public bool IsSameGroup(Unit a, Unit b) => IsSameGroup(a.hexPosition, b.hexPosition);

    /// <summary>
    /// ��� Bush�� Grouping�մϴ�.
    /// ������ Bush�� ���� �׷����� ���Դϴ�.
    /// BFS�� ����Ͽ� �����մϴ�.
    /// </summary>
    public void Group()
    {
        _bushesGroup.Clear();
        
        int group = 0;
        foreach (Bush bush in _bushes.Values)
        {
            if (_bushesGroup.ContainsKey(bush))
            {
                continue;
            }
            
            Queue<Bush> queue = new Queue<Bush>();
            queue.Enqueue(bush);
            
            while (queue.Count > 0)
            {
                Bush current = queue.Dequeue();
                _bushesGroup[current] = group;
                
                foreach (Bush neighbor in current.GetNeighbors())
                {
                    if (_bushesGroup.ContainsKey(neighbor))
                    {
                        continue;
                    }
                    
                    queue.Enqueue(neighbor);
                }
            }
            
            group++;
        }
    }
    
    //====================================================================================================
    
    public void AddNonHideUnit(Unit unit)
    {
        if (!_nonHideUnits.Contains(unit))
        {
            _nonHideUnits.Add(unit);
        }
        
        _player.CalculateUnitVision(unit);
    }
    
    public void RemoveNonHideUnit(Unit unit)
    {
        _nonHideUnits.Remove(unit);
        
        _player.CalculateUnitVision(unit);
    }
    
    public bool IsNonHideUnit(Unit unit)
    {
        return _nonHideUnits.Contains(unit);
    }
}