using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;

namespace CocoDoogy.CameraSwiper.UserInfo
{
    public class Tokens : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gameTicketText;
        [SerializeField] private TextMeshProUGUI inGameMoneyText;
        private FirebaseAuth auth;
        private FirebaseFirestore database;
        private ListenerRegistration listener;

        private void OnEnable()
        {
            auth = FirebaseAuth.DefaultInstance;
            database = FirebaseFirestore.DefaultInstance;

            ReadTokenData();
        }
        private void OnDisable()
        {
            listener?.Stop();
            listener = null;
        }

        private void ReadTokenData()
        {
            DocumentReference docRef = database
                .Collection("users")
                .Document(auth.CurrentUser.UserId)
                .Collection("private")
                .Document("data");

            listener = docRef.Listen(snapshot =>
                {
                    if (!snapshot.Exists)
                    {
                        gameTicketText.text = "0 / 15";
                        inGameMoneyText.text = "0";
                        return;
                    }

                    if (snapshot.TryGetValue<int>("gameTicket", out var gameTicket))
                    {
                        gameTicketText.text = $"{gameTicket.ToString()} / 15";
                    }
                    else gameTicketText.text = "0 / 15";

                    if (snapshot.TryGetValue<int>("inGameMoney", out var inGameMoney))
                    {
                        inGameMoneyText.text = $"{inGameMoney.ToString()}";
                    }
                    else inGameMoneyText.text = "0";
                }
            );
        }
    }
}
