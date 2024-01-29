using System.Collections.Generic;

public interface IItem
{
    /// <summary>
    /// Item의 고유 Index를 가져옵니다.
    /// </summary>
    public int GetIndex();
    
    /// <summary>
    /// Item의 이름을 가져옵니다.
    /// </summary>
    public string GetName();
    
    /// <summary>
    /// Item의 설명을 가져옵니다.
    /// </summary>
    public string GetDescription();
    
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
    /// 사용 가능한 아이템인지의 여부를 true / false로 반환합니다.
    /// </summary>
    public bool IsUsable();
    
    /// <summary>
    /// 인벤토리 내에서 중첩 가능한 아이템인지 여부를 반환합니다 (ex. 소모품).
    /// </summary>
    public bool IsStackable() => GetMaxStack() > 1;
    
    /// <summary>
    /// 아이템의 스택을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public int GetStack();
    public int GetMaxStack();
}