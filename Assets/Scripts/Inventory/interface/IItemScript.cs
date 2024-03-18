public interface IItemScript
{
    /// <summary>
    /// 아이템의 고유 Index를 가져옵니다.
    /// </summary>
    public int GetIndex();
    
    /// <summary>
    /// 아이템의 이름을 가져옵니다.
    /// </summary>
    public string GetName();
    
    /// <summary>
    /// 아이템의 설명을 가져옵니다.
    /// </summary>
    public string GetDescription(ItemData data);
}