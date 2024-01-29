public interface IItemAttribute
{
    /// <summary>
    /// 특성의 고유 Index를 가져옵니다.
    /// </summary>
    public int GetIndex();
    
    /// <summary>
    /// 특성의 이름을 가져옵니다.
    /// </summary>
    public string GetName();
    
    /// <summary>
    /// 특성의 설명을 가져옵니다.
    /// </summary>
    public string GetDescription();
}