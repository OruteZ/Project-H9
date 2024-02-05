using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Inventory.Inventory
{
    public class Inventory : ScriptableObject, IInventory
    {
        private List<IItem> items = new List<IItem>();

        public void AddItem(IItem item)
        {
            items.Add(item);
        }

        public void DeleteItem(IItem item)
        {
            items.Remove(item);
        }

        public IEnumerable<IItem> GetItems()
        {
            return items;
        }

        public int GetItemCount(IItem item)
        {
            return items.Count(i => i.Equals(item));
        }

        public int GetItemCount(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= items.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(itemIndex));
            }
            return items.Count(i => i.Equals(items[itemIndex]));
        }
    }
}