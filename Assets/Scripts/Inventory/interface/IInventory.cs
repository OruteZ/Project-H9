using System.Collections.Generic;
using UnityEngine.Events;

public interface IInventory
{
    /// <summary>
    /// 인벤토리에 아이템을 추가합니다.
    /// </summary>
    /// <param name="item"> 추가할 아이템 </param>
    public bool TryAddItem(IItem item);

    /// <summary>
    /// 인벤토리에서 아이템을 삭제합니다.
    /// </summary>
    /// <param name="item"> 삭제할 아이템 </param>
    /// <param name="cnt"> 삭제할 개수, 입력하지 않을 경우 1로 제한됩니다. </param>
    public void DeleteItem(IItem item, int index, int cnt = 1);

    /// <summary>
    /// 인벤토리에 있는 아이템들을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IItem> GetItems();

    /// <summary>
    /// 인벤토리에 있는 아이템들 중, 해당 아이템의 개수를 반환합니다.
    /// </summary>
    /// <param name="item"> 개수를 확인할 아이템 </param>
    /// <returns></returns>
    public int GetItemCount(IItem item);

    /// <summary>
    /// 인벤토리에 있는 아이템들 중, 해당 아이템의 개수를 반환합니다.
    /// </summary>
    /// <param name="itemIndex"> 개수를 확인할 아이템의 고유번호 </param>
    /// <returns></returns>
    public int GetItemCount(int itemIndex);
    
    public static UnityEvent OnInventoryChanged = OnInventoryChanged ?? new UnityEvent();
    public static UnityEvent<ItemData> OnGetItem = OnGetItem ?? new UnityEvent<ItemData>(); 
    public static UnityEvent<ItemData> OnUseItem = OnUseItem ?? new UnityEvent<ItemData>();
}