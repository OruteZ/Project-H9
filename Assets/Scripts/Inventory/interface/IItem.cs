using System.Collections.Generic;


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
    public void Use();
    
    /// <summary>
    /// 아이템의 스택 개수를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int GetStackCount();
    
    public bool TryEquip();
    public bool TrySplit(int count, out IItem newItem);
}