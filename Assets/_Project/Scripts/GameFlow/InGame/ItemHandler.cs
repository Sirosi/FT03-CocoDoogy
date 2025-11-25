using CocoDoogy.Data;
using System;
using System.Collections.Generic;

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
    }
}
