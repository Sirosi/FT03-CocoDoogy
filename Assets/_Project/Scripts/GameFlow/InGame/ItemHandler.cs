using CocoDoogy.Data;
using CocoDoogy.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class ItemHandler 
    {
        public static Dictionary<ItemData, bool> UsedItems { get; private set; } = new();
        
        public static event Action<ItemData, bool> OnValueChanged;
        
        public static void SetValue(ItemData key, bool value)
        {
            if (!UsedItems.ContainsKey(key) || UsedItems[key] != value)
            {
                UsedItems[key] = value;
                OnValueChanged?.Invoke(key, value);
            }
        }

        public static void UseItem()
        {
            foreach (var itemData in UsedItems)
            {
                Debug.Log("아이템 사용");
                if (!itemData.Value)
                {
                    _ = FirebaseManager.UseItemAsync(itemData.Key.itemId);
                }
            }
        }
    }
}
