using CocoDoogy.Data;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.Popup;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.Network.Login
{
    // TODO : 일단 구글 로그인 테스트 용으로 생성한 스크립트 나중에 FirebaseManager나 다른 스크립트와 합쳐서 사용하도록 수정 예정
    public class LoginUI : MonoBehaviour
    {
        private FirebaseManager Firebase => FirebaseManager.Instance;
        private LoginViewModel loginVM;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI errorLog;
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private TextMeshProUGUI userEmail;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject profilePanel;
        
        [SerializeField] private Button anonymousLoginButton;
        [SerializeField] private Button linkGoogleButton;
        [SerializeField] private Button gameTicketUseButton;
        [SerializeField] private Button findUserByNicknameButton;
        private void Start()
        {
            var authProvider = new AuthProvider();
            loginVM = new LoginViewModel(authProvider);

            loginButton.interactable = false;
            anonymousLoginButton.interactable = false;
            
            Firebase.SubscribeOnFirebaseInitialized(() =>
            {
                authProvider.InitGoogleSignIn();
                loginButton.interactable = true;
                anonymousLoginButton.interactable = true;
            });
            
            loginVM.OnUserChanged += OnUserLoggedIn;
            loginVM.OnLoggedOut += OnUserLoggedOut;
            loginVM.OnErrorChanged += OnLoginError;
            
            loginButton.onClick.AddListener(() => loginVM.SignIn());
            logoutButton.onClick.AddListener(() => loginVM.SignOut());
            
            anonymousLoginButton.onClick.AddListener(() => loginVM.SignInAnonymously()); 
            linkGoogleButton.onClick.AddListener(() => loginVM.LinkGoogleAccount());
            gameTicketUseButton.onClick.AddListener(OnConsumeTicket);
            findUserByNicknameButton.onClick.AddListener(OnClickFindUserByNicknameAsync);

            DataManager.Instance.OnPrivateUserDataLoaded += RefreshFriendRequestList;
        }

        

        #region < 로그인 성공, 실패, 로그아웃 UI >
        private void OnUserLoggedIn(FirebaseUser loginUser)
        {
            userName.text = loginUser.IsAnonymous ? "Anonymous User" : loginUser.DisplayName; 
            userEmail.text = loginUser.IsAnonymous ? "(No Email)" : loginUser.Email;
            loginPanel.SetActive(false);
            profilePanel.SetActive(true);
            
            // TODO : 일정 시간마다 티켓 정상적으로 획득되는지 테스트 용
            //ticketHandler.StartRecharge();
            StartCoroutine(Firebase.UpdateLocalTimerCoroutine());
        }
        
        private void OnUserLoggedOut()
        {
            userName.text = string.Empty;
            userEmail.text = string.Empty;
            loginPanel.SetActive(true);
            profilePanel.SetActive(false);
        }
        
        private void OnLoginError(string errorMessage)
        {
            errorLog.text = errorMessage;
        }
        #endregion

        private async void OnConsumeTicket()
        {
            bool check = await Firebase.UseTicketAsync();
            if (check)
            {
                Debug.Log("성공");
            }
            else
            {
                Debug.Log("실패");
            }
        }

        #region < 친구 추가 기능 >

        private async void OnClickFindUserByNicknameAsync()
        {
            string nickname = await GetValidNickNameAsync();
            string uid = await Firebase.FindUserByNicknameAsync(nickname);
            var result = await Firebase.CallFriendFunctionAsync("sendFriendsRequest", uid, "친구 요청 보내기 실패");
            bool success = (bool)result["success"];
            if (success)
            {
                Debug.Log("정상적으로 친구 초대가 보내졌습니다");
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                Debug.LogWarning($"{reason}");
            }
        }

        private async Task<string> GetValidNickNameAsync()
        {
            string nickname = null;

            while (string.IsNullOrWhiteSpace(nickname))
            {
                nickname = await ShowNicknameInputDialogAsync();

                if (string.IsNullOrWhiteSpace(nickname))
                {
                    await ShowMessageAsync("알림", "닉네임은 비워둘 수 없습니다!");
                }
            }

            return nickname;
        }

        private Task ShowMessageAsync(string title, string message)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            MessageDialog.ShowMessage(title, message, DialogMode.Confirm, (_) => taskCompletionSource.SetResult(true));

            return taskCompletionSource.Task;
        }

        private Task<string> ShowNicknameInputDialogAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            MessageDialog.ShowInputBox(
                "친구 초대",
                "친구 초대를 원하시는 유저의 닉네임을 입력해주세요.",
                (nickname) => taskCompletionSource.SetResult(nickname));

            return taskCompletionSource.Task;
        }
        #endregion

        #region < 친구 추가 수락 기능>
        [SerializeField] private Transform requestListParent;
        [SerializeField] private GameObject friendRequestItemPrefab;
        private async void RefreshFriendRequestList()
        {
            foreach (Transform child in requestListParent)
                Destroy(child.gameObject);

            var requestDict = await Firebase.GetFriendRequestsAsync("friendReceivedList");
            foreach (var kvp in requestDict)
            {
                string uid = kvp.Key;
                string nickname = kvp.Value;
                var item = Instantiate(friendRequestItemPrefab, requestListParent);
                item.GetComponent<FriendRequestItem>().ReceivedInit(nickname, uid, OnAcceptRequest, OnRejectRequest);
            }
        }
        private async void OnAcceptRequest(string uid)
        {
            var result = await Firebase.CallFriendFunctionAsync("receiveFriendsRequest", uid, "친구 요청 수락 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                await ShowMessageAsync("성공", "친구 요청을 수락했습니다!");
                RefreshFriendRequestList();
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                await ShowMessageAsync("실패", $"친구 요청 수락 실패: {reason}");
            }
        }

        private async void OnRejectRequest(string uid)
        {
            var result = await Firebase.CallFriendFunctionAsync("rejectFriendsRequest", uid, "친구 요청 거절 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                await ShowMessageAsync("성공", "친구 요청을 거절했습니다.");
                RefreshFriendRequestList();
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                await ShowMessageAsync("실패", $"친구 요청 거절 실패: {reason}");
            }
        }
        #endregion
    }
}