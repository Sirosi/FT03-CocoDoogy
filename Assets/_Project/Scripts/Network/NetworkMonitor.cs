using CocoDoogy.Core;
using CocoDoogy.UI.Popup;
using CocoDoogy.Utility.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Network
{
    public class NetworkMonitor : Singleton<NetworkMonitor>
    {
        public bool IsConnected { get; private set; } = true;

        private bool lastState = true;

        private MessageDialog isPopup;
        private FirebaseLoading loadingUI;
        private bool isChecking = false;
        protected override void Awake()
        {
            base.Awake();
            InvokeRepeating(nameof(CheckNetwork), 0f, 2f);
        }

        private async UniTask CheckNetwork()
        {
            bool nowConnected =
                Application.internetReachability != NetworkReachability.NotReachable;

            if (nowConnected != lastState)
            {
                lastState = nowConnected;
                IsConnected = nowConnected;

                if (!nowConnected)
                {
                    Debug.Log("인터넷 끊김! 재연결 시도중…");

                    loadingUI = FirebaseLoading.ShowLoading();
                    
                    bool reconnected = await WaitForReconnect(10f);

                    if (reconnected)
                    {
                        loadingUI?.Hide();
                        Debug.Log("인터넷 재연결!");
                        return;
                    }

                    loadingUI?.Hide();
                    if (!isPopup)
                    {
                        isPopup = MessageDialog.ShowMessage(
                            "인터넷 연결 실패",
                            "인터넷 연결이 끊겼습니다.\n인터넷 상태를 확인해주세요.",
                            DialogMode.Confirm, 
                            ThrowIntro
                        );
                    }
                }
                else
                {
                    Debug.Log("인터넷 연결됨");
                }
            }
        }

        /// <summary>
        /// 특정 시간 동안 재연결을 기다림
        /// </summary>
        private async UniTask<bool> WaitForReconnect(float maxWaitSeconds)
        {
            float timer = 0;

            while (timer < maxWaitSeconds)
            {
                await UniTask.Delay(500);
                timer += 0.5f;

                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    IsConnected = true;
                    lastState = true;
                    return true;
                }
            }

            return false; 
        }

        private void ThrowIntro(CallbackType callbackType)
        {
            if(callbackType == CallbackType.Yes)
            {
                isPopup = null;
                Loading.LoadScene("Intro");
            }
        }
    }
}