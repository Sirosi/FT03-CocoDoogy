using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy
{
    public class InGameItemUI : MonoBehaviour
    {
        // TODO: 변경 사항
        // 1. 게임에 들어오기 전 UI에서는 아이템의 수량만 보여주고 아이템을 눌렀을 때 아이템 설명과 상점으로 가는 버튼이 있는 Popup 띄움.(Clear)
        // 2. 인게임에 들어가서 우측 중앙에 아이템 그룹이 있고 거기서 아이템을 눌렀을 때 1과 마찬가지고 아이템 정보가 나오는데 상점 버튼 대신 사용버튼, 취소버튼
        // 3. 사용한 아이템은 버튼이 비활성화 된 상태로 유지

        [SerializeField] private InGameItemButton[] itemButtons;

        private void Awake()
        {
            for (int i = 0; i < itemButtons.Length; i++)
            {
                itemButtons[i].ItemData = DataManager.Instance.ItemData[i];
                itemButtons[i].OnClickEvent += ShowInfo;
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
