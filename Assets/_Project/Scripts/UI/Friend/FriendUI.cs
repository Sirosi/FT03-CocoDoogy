using CocoDoogy.Network.UI;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class FriendUI : UIPanel
    {
        [Header("UI Elements")] 
        [SerializeField] private RectTransform friendsWindow;
        [SerializeField] private GameObject searchWindowBg;

        [Header("Menu Buttons")] 
        [SerializeField] private CommonButton closeButton;
        [SerializeField] private CommonButton friendsInfoButton;
        [SerializeField] private CommonButton friendsRequestButton;
        [SerializeField] private CommonButton friendsSentButton;

        [Header("Popup Buttons")] 
        [SerializeField] private CommonButton searchFriendButton;
        [SerializeField] private CommonButton sendAllButton;

        [Header("Tabs")] 
        [SerializeField] private FriendsInfoPanel friendsInfoPanel;
        [SerializeField] private ReceivedRequestPanel receivedRequestPanel;
        [SerializeField] private SentRequestPanel sentRequestPanel;
        
        public FriendsInfoPanel FriendsInfoPanel => friendsInfoPanel;
        public ReceivedRequestPanel ReceivedRequestPanel => receivedRequestPanel;
        public SentRequestPanel SentRequestPanel => sentRequestPanel;
        
        /// <summary>
        /// 현재 열려있는 친구 탭 패널
        /// </summary>
        private RequestPanel currentActivePanel; 
        
        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            friendsInfoButton.onClick.AddListener(OnClickFriendInfoButton);
            friendsRequestButton.onClick.AddListener(OnClickFriendRequestButton);
            friendsSentButton.onClick.AddListener(OnClickFriendSentButton);
            searchFriendButton.onClick.AddListener(OnClickFriendSearch);
        }

        private void OnEnable()
        {
            InitTabs();
            WindowAnimation.CloseWindow(searchWindowBg.transform);
        }

        public override void OpenPanel() =>  gameObject.SetActive(true);
        protected override void ClosePanel() => WindowAnimation.SwipeWindow(friendsWindow);
        private void OnClickFriendSearch() =>searchWindowBg.SetActive(true);
        
        #region ChangeTabs
        private void InitTabs()
        {
            currentActivePanel = friendsInfoPanel;
            friendsInfoPanel.gameObject.SetActive(true);
            receivedRequestPanel.gameObject.SetActive(false);
            sentRequestPanel.gameObject.SetActive(false);
        }
        private void OnClickFriendInfoButton() =>ToggleTab(friendsInfoPanel);
        private void OnClickFriendRequestButton() => ToggleTab(receivedRequestPanel);
        private void OnClickFriendSentButton() =>ToggleTab(sentRequestPanel);

        private void ToggleTab(RequestPanel targetPanel)
        {
            if (currentActivePanel == targetPanel) return;

            if (currentActivePanel != null)
            {
                currentActivePanel.gameObject.SetActive(false);
            }
            
            targetPanel.gameObject.SetActive(true);
            currentActivePanel = targetPanel;
        }
        #endregion
        
    }
}