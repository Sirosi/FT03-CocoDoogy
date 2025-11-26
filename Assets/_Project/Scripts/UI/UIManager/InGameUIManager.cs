using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.Utility.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.UIManager
{
    public class InGameUIManager : Singleton<InGameUIManager>
    {
        [Header("Main Buttons")]
        [SerializeField] private CommonButton openSettingsButton;
        [SerializeField] private CommonButton openPauseButton;


        [Header("PauseWindow Buttons")]
        [SerializeField] private CommonButton resumeButton;
        [SerializeField] private CommonButton resetButton;
        [SerializeField] private CommonButton openQuitButton;
        [SerializeField] private Slider volumeSlider;


        [Header("Option UI Elements")]
        [SerializeField] private RectTransform settingsWindow;
        [SerializeField] private RectTransform pauseWindow;

        protected override void Awake()
        {
            base.Awake();
            openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
            openPauseButton.onClick.AddListener(OnOpenPauseButtonClicked);

            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            resetButton.onClick.AddListener(OnOpenResetButtonClicked);
            openQuitButton.onClick.AddListener(OnOpenQuitButtonClicked);
        }


        private void OnOpenSettingsButtonClicked()
        {
            if (!settingsWindow.gameObject.activeSelf)
            {
                settingsWindow.gameObject.SetActive(true);
            }
        }

        private void OnOpenPauseButtonClicked()
        {
            pauseWindow.gameObject.SetActive(true);

            SfxManager.PlayDucking(0.6f);
        }

        private void OnResumeButtonClicked()
        {
            pauseWindow.gameObject.SetActive(false);

            SfxManager.StopDucking();
        }

        private void OnOpenResetButtonClicked()
        {
            MessageDialog.ShowMessage("다시하기", "모든 걸 버리고 다시 시작할까요?", DialogMode.YesNo, ResetOrNot);
        }

        private void OnOpenQuitButtonClicked()
        {
            MessageDialog.ShowMessage("나가기", "주인은 다음에 찾을까요?", DialogMode.YesNo, QuitOrNot);
        }


        private void ResetOrNot(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                OnResetButtonClicked();
            }
        }

        private void QuitOrNot(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                OnQuitButtonClicked();
            }
        }


        public async void OnResetButtonClicked()
        {
            bool isReady = await FirebaseManager.UseTicketAsync();
            if (isReady)
            {
                SfxManager.StopDucking();
                Loading.LoadScene("InGame");
            }
            else
            {
                // TODO : 티켓이 부족하면 메세지를 띄우게만 해뒀는데 여기에서 상점으로 연결까지 할 수도?
                MessageDialog.ShowMessage(
                    "티켓 부족",
                    "티켓이 부족하여 게임을 진행할 수 없습니다.",
                    DialogMode.Confirm,
                    null);
            }
        }


        public void OnQuitButtonClicked()
        {
            SfxManager.StopDucking();
            Loading.LoadScene("Lobby");
        }
    }
}
