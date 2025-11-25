using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.StageSelect;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy
{
    public class ProfileUI : UIPanel
    {
        [SerializeField] private RectTransform profileWindow;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI recordText;
        [SerializeField] private Button closeButton;

        private FirebaseManager Firebase => FirebaseManager.Instance;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        private void OnEnable()
        {
            _ = RefreshUIAsync();
        }
        
        public override void ClosePanel()
        {
            WindowAnimation.CloseWindow(profileWindow);
            PageCameraSwiper.IsSwipeable = true;
        }


        private async Task RefreshUIAsync()
        {
            var docRef = Firebase.Firestore
                .Collection("users").Document(Firebase.Auth.CurrentUser.UserId)
                .Collection("public").Document("profile");
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var data = snapshot.ToDictionary();
                nicknameText.text = data["nickName"].ToString();
                recordText.text =
                    $"{StageSelectManager.LastClearedStage.theme} 테마 {StageSelectManager.LastClearedStage.level} 스테이지";
            }
            else
            {
                Debug.Log("해당 문서가 존재하지 않습니다.");
            }
        }
    }
}