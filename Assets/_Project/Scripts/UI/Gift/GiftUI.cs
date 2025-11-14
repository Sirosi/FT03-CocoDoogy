using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace CocoDoogy.UI.Gift
{
    public class GiftUI : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button closeThisButton;
        [SerializeField] private CommonButton getAllButton;
        [SerializeField] private CommonButton confirmButton;
        
        [Header("UI Elements")]
        [SerializeField] private RectTransform giftWindow;
        [SerializeField] private RectTransform getGiftWindow;
        
        [SerializeField] private RectTransform container;
        [SerializeField] private GiftItem prefabItem;
        
        FirebaseManager Firebase => FirebaseManager.Instance;
        
        private void Awake()
        {
            closeThisButton.onClick.AddListener(ClosePanel);
            getAllButton.onClick.AddListener(OnGetAllButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        public override void ClosePanel() => WindowAnimation.SwipeWindow(giftWindow);
        public void SubscriptionEvent() => _ = RefreshPanelAsync();
        private void OnGetAllButtonClicked() => getGiftWindow.gameObject.SetActive(true);
        private void OnConfirmButtonClicked() =>  WindowAnimation.CloseWindow(getGiftWindow);

        private async Task RefreshPanelAsync()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            var requestDict = await Firebase.GetGiftListAsync();
            foreach (var kvp in requestDict)
            {
                var item = Instantiate(prefabItem, container);
                item.GetComponent<GiftItem>().Init(kvp["fromNickname"].ToString(),
                    kvp["giftId"].ToString(),
                    kvp["giftCount"].ToString()
                    ,OnTakePresentAsync);
            }
        }

        private async void OnTakePresentAsync(string itemType)
        {
            var result = await Firebase.TakePresentRequestAsync("takePresentRequest", itemType, "선물 받기 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("선물 받기 성공","선물 받기를 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("선물 받기 실패", reason, DialogMode.Confirm, null);
            }
        }
    }
}
