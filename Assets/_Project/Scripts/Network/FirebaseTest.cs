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

            Firebase.SubscribeOnFirebaseInitialized(() =>
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

            // TODO : 일정 시간마다 티켓 정상적으로 획득되는지 테스트 용
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