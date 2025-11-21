using CocoDoogy.Network.Login;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.Network.UI
{
    public class FirebaseTest : MonoBehaviour
    {
        private FirebaseManager Firebase => FirebaseManager.Instance;
        private LoginViewModel loginVM;

        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject lobbyPanel;

        [Header("Buttons")]
        [SerializeField] private Button anonymousLoginButton;
        [SerializeField] private Button googleLoginButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button useGameTicketButton;
        private void Start()
        {
            var authProvider = new AuthProvider();
            loginVM = new LoginViewModel(authProvider);

            googleLoginButton.interactable = false;
            anonymousLoginButton.interactable = false;

            FirebaseManager.SubscribeOnFirebaseInitialized(() =>
            {
                authProvider.InitGoogleSignIn();
                googleLoginButton.interactable = true;
                anonymousLoginButton.interactable = true;
            });

            loginVM.OnUserChanged += OnUserLoggedIn;
            loginVM.OnLoggedOut += OnUserLoggedOut;
            loginVM.OnErrorChanged += OnLoginError;

            googleLoginButton.onClick.AddListener(() => loginVM.SignIn());
            logoutButton.onClick.AddListener(() => loginVM.SignOut());
            useGameTicketButton.onClick.AddListener(OnConsumeTicketAsync);
            anonymousLoginButton.onClick.AddListener(() => loginVM.SignInAnonymously());
        }


        private void OnUserLoggedIn(FirebaseUser loginUser)
        {
            loginPanel.SetActive(false);

            // 로그인에 성공 후 실행되며 게임이 종료될 때까지 실행
            // 일정 시간마다 실행되며 해당 시간이 지나면 티켓이 생성되도록 함.
            StartCoroutine(Firebase.UpdateLocalTimerCoroutine());
        }

        private void OnUserLoggedOut()
        {
            loginPanel.SetActive(true);
        }

        private void OnLoginError(string errorMessage)
        {
            Debug.LogError(errorMessage);
        }
        private async void OnConsumeTicketAsync()
        {
            bool check = await Firebase.UseTicketAsync();
            if (check)
            {
                Debug.Log("success");
            }
            else
            {
                Debug.Log("fail");
            }
        }
    }
}