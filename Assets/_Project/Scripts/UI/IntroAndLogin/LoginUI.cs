using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.Network.Login;
using CocoDoogy.UI.UIManager;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CocoDoogy.UI.IntroAndLogin
{
    public class LoginUI : UIPanel
    {
        /// <summary>
        /// 구글로그인을 하는 버튼
        /// </summary>
        [SerializeField] private Button googleLoginButton;
        /// <summary>
        /// 익명로그인을 하는 버튼 
        /// </summary>
        [SerializeField] private Button anonymousLoginButton;
        
        private LoginViewModel loginVM;
        
        private FirebaseManager Firebase => FirebaseManager.Instance;


        private void Awake()
        {
            Init();
        }
        
        /// <summary>
        /// 로그인을 위한 초기화를 하는 메서드
        /// </summary>
        private void Init()
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
            loginVM.OnErrorChanged += OnLoginError;
            
            googleLoginButton.onClick.AddListener(() => loginVM.SignIn());
            anonymousLoginButton.onClick.AddListener(() => loginVM.SignInAnonymously()); 
        }
        
        public override void ClosePanel() => gameObject.SetActive(false);
        
        /// <summary>
        /// 로그인에 성공 시 Lobby로 씬을 이동시키는 메서드
        /// </summary>
        /// <param name="loginUser"></param>
        private void OnUserLoggedIn(FirebaseUser loginUser)
        {
            SceneManager.LoadScene("UIConnectTest");
        }
        
        private void OnLoginError(string errorMessage)
        {
            
        }
    }
}