using CocoDoogy.Network;
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
        [SerializeField] private TextMeshProUGUI cashMoneyText;
        
        private FirebaseManager Firebase => FirebaseManager.Instance;

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
                ticketCountText.text = $"{ticketCount.ToString()} / 5";
                cashMoneyText.text = data["cashMoney"].ToString();
            }
            else
            {
                Debug.Log("해당 문서가 존재하지 않습니다.");
            }
        }
    }
}