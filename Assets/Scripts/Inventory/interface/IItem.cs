using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IItem
{
    /// <summary>
    /// Item의 정보를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public ItemData GetData();
    
    /// <summary>
    /// 아이템에 붙어있는 Attribute들을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IItemAttribute> GetAttributes();
    
    /// <summary>
    /// 아이템을 사용할 수 있을경우, 사용합니다.
    /// </summary>
    public bool Use(Unit user);
    public bool Use(Unit user, Vector3Int target);
    
    /// <summary>
    /// 아이템의 스택 개수를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int GetStackCount();
    
    /// <summary>
    /// 아이템의 스택 개수를 재설정 합니다.
    /// </summary>
    public void SetStackCount(int count);
    
    public bool TryEquip();
    public bool TrySplit(int count, out IItem newItem);

    /// <summary>
    /// 이 아이템을 사용하는 행동이 Immediate하게 실행되어야 하는지 여부를 반환합니다.
    /// </summary>
    public bool IsImmediate();
    
    /// <summary>
    /// 해당 아이템이 사용가능한지 여부를 반환합니다.
    /// </summary>
    public bool IsUsable();
    
    public UnityEvent OnItemChanged { get; }

    public static IItem operator -(IItem item, int modify)
    {
        item.SetStackCount(Mathf.Max(item.GetStackCount() - modify, 0));
        return item;
    }
}