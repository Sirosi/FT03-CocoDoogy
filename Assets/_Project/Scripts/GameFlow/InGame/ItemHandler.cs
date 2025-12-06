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
         
        /// <summary>
        /// 스테이지를 클리어 혹은 실패 후 아이템을 사용한 적이 있다면 해당 아이템을 DB에서 -1 하고
        /// </summary>
        /// <param name="openPopup"></param>
        public static async void UseItem(Action openPopup = null)
        {
            try
            {
                foreach (var itemData in UsedItems)
                {
                    if (!itemData.Value)
                    {
                        _ = await FirebaseManager.UseItemAsync(itemData.Key.itemId);
                        DataManager.Instance.CurrentItem[itemData.Key]--;
                    }
                }
                openPopup?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"아이템 구매 버그: {e.Message}");
            }
        }
    }
}
