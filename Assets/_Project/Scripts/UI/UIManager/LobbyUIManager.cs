using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.Gift;
using CocoDoogy.UI.Shop;
using CocoDoogy.UI.UserInfo;
using System.Collections;
using UnityEngine;

namespace CocoDoogy.UI.UIManager
{
    public class LobbyUIManager : Singleton<LobbyUIManager>
    {
        [Header("Lobby UI Panels")]
        [SerializeField] private ProfileUI profilePanel;
        [SerializeField] private FriendUI friendPanel;
        [SerializeField] private GiftUI giftPanel;
        [SerializeField] private SettingsUI settingsPanel;
        [SerializeField] private ShopUI shopPanel; 
        [SerializeField] private InfoUI infoPanel;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton profileButton;
        [SerializeField] private CommonButton friendsButton;
        [SerializeField] private CommonButton giftsButton;
        [SerializeField] private CommonButton settingsButton;
        [SerializeField] private CommonButton shopButton;
        [SerializeField] private CommonButton startButton;
        
        protected override void Awake()
        {
            base.Awake();
            profileButton.onClick.AddListener(OnClickProfileButton);
            friendsButton.onClick.AddListener(OnClickFriendButton);
            giftsButton.onClick.AddListener(OnClickGiftButton);
            settingsButton.onClick.AddListener(OnClickSettingButton);
            shopButton.onClick.AddListener(OnClickShopButton);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private IEnumerator Start()
        {
            // 해당 이벤트 추가는 로그인 후 되어야 되므로 UIManager에서 구독, 나중에 문제되면 DataManager.Instance가 null 아닐때 async로 변경해서 사용
            yield return new WaitUntil(() => DataManager.Instance != null);
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.FriendsInfoPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.ReceivedRequestPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded += friendPanel.SentRequestPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded += giftPanel.SubscriptionEvent;
            DataManager.Instance.OnPrivateUserDataLoaded += infoPanel.SubscriptionEvent;
        }
        private void OnClickProfileButton() => profilePanel.OpenPanel();
        private void OnClickFriendButton() => friendPanel.OpenPanel();
        private void OnClickGiftButton() =>  giftPanel.OpenPanel();
        private void OnClickSettingButton() => settingsPanel.OpenPanel();
        private void OnClickShopButton() => shopPanel.OpenPanel();
        
        private void OnStartButtonClicked() => Loading.LoadScene("StageSelectUITest");

    }
}
