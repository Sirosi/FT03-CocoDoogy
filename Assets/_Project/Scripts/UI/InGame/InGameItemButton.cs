using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CocoDoogy
{
    public class InGameItemButton : MonoBehaviour
    {
        private CommonButton button;

        public ItemData ItemData { get; set; }

        public Action<InGameItemButton, ItemData> OnClickEvent;

        private void Awake()
        {
            if (!button)
            {
                button = GetComponent<CommonButton>();
            }

            button.onClick.AddListener(() => OnClickEvent?.Invoke(this, ItemData));
        }

        /// <summary>
        /// 아이템 사용 메서드
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public async void UseItem(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            
            IDictionary<string, object> result = await FirebaseManager.UseItemAsync(itemData.itemId);
            bool success = (bool)result["success"];
            if (success)
            {
                // 아이템 1회 사용 후 더이상 사용하지 못하게 변경
                button.interactable = false;
                // TODO : 아이템 사용 시 인게임 기능 추가 
            }
            else
            {
                Debug.Log("아이템 사용 실패");
            }
        }

        /// <summary>
        /// 아이템 구매 메서드 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public async Task PurchaseAsync(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            try
            {
                var result = await FirebaseManager.PurchaseWithCashMoneyAsync(itemData.itemId, 1);

                bool success = result.ContainsKey("success") && (bool)result["success"];

                if (success)
                {
                    Debug.Log($"구매 성공: {itemData.itemName} ({1})");
                    DataManager.Instance.CurrentItem[itemData] += 1;
                }
                else
                {
                    string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                    Debug.LogWarning($"구매 실패: {reason}");
                    MessageDialog.ShowMessage("구매실패", reason, DialogMode.Confirm, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"구매 실패: {e.Message}");
            }
        }
    }
}
