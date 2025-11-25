using CocoDoogy.Network;
using CocoDoogy.UI.UIManager;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.UserInfo
{
    public class InfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ticketCountText;
        [SerializeField] private TextMeshProUGUI realtimeTicket;
        [SerializeField] private TextMeshProUGUI cashMoneyText;
        [SerializeField] private CommonButton stampShopButton;
        [SerializeField] private CommonButton jemShopButton;
        
        private FirebaseManager Firebase => FirebaseManager.Instance;

        private void Awake()
        {
            stampShopButton.onClick.AddListener((() => LobbyUIManager.Instance.ShopUI.OpenStampShopUI()));
            jemShopButton.onClick.AddListener((() => LobbyUIManager.Instance.ShopUI.OpenJemShopUI()));
        }
        public void SubscriptionEvent() => _ = RefreshUIAsync();
        
        private async Task RefreshUIAsync()
        {
            var docRef = Firebase.Firestore
                .Collection("users").Document(Firebase.Auth.CurrentUser.UserId)
                .Collection("private").Document("data");
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var data = snapshot.ToDictionary();
                
                long ticketCount = (long)data["gameTicket"] + (long)data["bonusTicket"];
                if (ticketCount > 99)
                {
                    ticketCountText.text = $"99+ / 5";
                }
                else
                {
                    ticketCountText.text = $"{ticketCount.ToString()} / 5";
                }
                cashMoneyText.text = data["cashMoney"].ToString();
                
                
                realtimeTicket.text =  $"현재 코코 도장 : {ticketCount.ToString()}개\n5분마다 하나씩 충전됩니다.";
            }
            else
            {
                Debug.Log("해당 문서가 존재하지 않습니다.");
            }
        }
    }
}