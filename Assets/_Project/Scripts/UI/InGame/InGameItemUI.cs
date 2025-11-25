using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy
{
    public class InGameItemUI : MonoBehaviour
    {
        [SerializeField] private InGameItemButton[] itemButtons;
        
        private void Awake()
        {
            ItemHandler.OnValueChanged += (item, value) =>
            {
                foreach (var button in itemButtons)
                {
                    if (button.ItemData == item)
                    {
                        button.Button.interactable = value;
                    }
                }
            };
        }

        private void Start()
        {
            for (int i = 0; i < itemButtons.Length; i++)
            {
                ItemData itemData = DataManager.Instance.ItemData[i];
                itemButtons[i].ItemData = itemData;
                itemButtons[i].OnClicked += ShowInfo;
                ItemHandler.SetValue(itemData, true);
            }
        }

        private void ShowInfo(InGameItemButton button, ItemData itemData)
        {
            if (itemData is null) return;

            // 아이템이 1개 이상 존재 하면 사용할 수 있도록
            if (DataManager.Instance.CurrentItem[itemData] > 0)
            {
                InfoDialog.ShowInfo("아이템 정보", "아이템 설명", itemData.itemDescription, itemData.itemSprite, DialogMode.YesNo,
                    (type => button.UseItem(type, itemData)));
            }
            else // 아이템이 존재하지 않으면 구매할 수 있도록
            {
                InfoDialog.ShowInfo("아이템 정보", "아이템 설명", itemData.itemDescription, itemData.itemSprite, DialogMode.Confirm,
                    (type => _ = button.PurchaseAsync(type, itemData)));
            }
        }
    }
}
