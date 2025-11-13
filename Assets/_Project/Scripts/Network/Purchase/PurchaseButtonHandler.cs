using CocoDoogy.Network.UI;
using CocoDoogy.UI;
using CocoDoogy.UI.UserInfo;
using System;
using UnityEngine;
using UnityEngine.UI;


// TODO : 지금은 사용하지 않는거 ShopHandler에서 이벤트를 넣어주는 방식으로 변경
namespace CocoDoogy.Network.Purchase
{
    public class PurchaseButtonHandler : MonoBehaviour
    {
        [SerializeField] private CommonButton purchaseButton;
        [SerializeField] private string itemId;
        
        private ShopItemUI shopUI;

        public ShopItemUI ShopItemUI { set => shopUI = value; }

        public string ItemId
        {
            set => itemId = value;
        }
        
        private void Awake()
        {
            purchaseButton ??= GetComponent<CommonButton>();
            purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
        }

        private void OnPurchaseButtonClick()
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogError("아이템ID가 설정되지 않았습니다.");
                return;
            }
            // PurchaseItemAsync(itemId);
        }

        // private async void PurchaseItemAsync(string id)
        // {
        //     try
        //     {
        //         var result = await FirebaseManager.Instance.PurchaseWithCashMoneyAsync(id);
        //
        //         bool success = (bool)result["success"];
        //         if (success)
        //         {
        //             //Debug.Log($"구매 성공: {result["itemId"]} / {result["price"]}");
        //         }
        //         else
        //         {
        //             string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
        //             Debug.LogWarning($"구매 실패: {reason}");
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"구매 에러: {e.Message}");
        //     }
        // }
    }
}