using System;
using System.Collections.Generic;
using UnityEngine;
    [Serializable]
    public abstract class Item : IItem
    {
        private readonly ItemData _data;
        private readonly List<IItemAttribute> _attributes;
        protected int stackCount;
        public Item(ItemData data)
        {
            _data = data;
            _attributes = new List<IItemAttribute>();
            stackCount = 1;
        }

        public static Item CreateItem(ItemData data)
        {
            //read item type
            switch (data.type)
            {
                case ItemType.Weapon:
                    return new WeaponItem(data);
            }
            
            return null;
        }

        public ItemData GetData() => _data;
        public IEnumerable<IItemAttribute> GetAttributes() => _attributes;
        public bool Use(Unit user) => Use(user, Vector3Int.zero);
        public abstract bool Use(Unit user, Vector3Int target);
        
        public int GetStackCount() => stackCount;
        public abstract bool TryEquip();

        public bool TrySplit(int count, out IItem newItem)
        {
            newItem = null;
            if (count > 0 && _data.stackAble && count < stackCount)
            {
                stackCount -= count;
                newItem = (Item)MemberwiseClone();
                ((Item)newItem).stackCount = count;
                return true;
            }
            return false;
        }

        public static Item operator +(Item item1, Item item2)
        {
            if (item1.GetData().id == item2.GetData().id && item1.GetData().stackAble)
            {
                item1.stackCount += item2.stackCount;
                return item1;
            }
            Debug.LogError("Cannot stack items");
            return null;
        }
    }