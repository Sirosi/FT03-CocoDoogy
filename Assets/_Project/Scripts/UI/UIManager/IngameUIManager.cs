using CocoDoogy.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.UIManager
{
    public class IngameUIManager : MonoBehaviour
    {
        [Header("SettingsWindow Buttons")]
        [SerializeField] private CommonButton openSettingsButton;
        [SerializeField] private CommonButton openPauseButton;
        [SerializeField] private CommonButton openQuitButton;
        
        
        [Header("PauseWindow Buttons")]
        [SerializeField] private CommonButton resumeButton;
        [SerializeField] private CommonButton pauseQuitButton;
        [SerializeField] private Slider volumeSlider;
        
        [Header("QuitWindow Buttons")]
        [SerializeField] private CommonButton backButton;
        [SerializeField] private CommonButton quitButton;
        
        [Header("CompleteWindow Buttons")]
        [SerializeField] private CommonButton againButton;
        [SerializeField] private CommonButton nextStageButton;
        
        [Header("DefeatWindow Buttons")]
        [SerializeField] private CommonButton restartButton;
        [SerializeField] private CommonButton homeButton;
        
        [Header("Option UI Elements")]
        [SerializeField] private RectTransform settingsWindow;
        [SerializeField] private RectTransform resetWindow;
        [SerializeField] private RectTransform pauseWindow;
        [SerializeField] private RectTransform quitWindow;

        void Awake()
        {
            openSettingsButton.onClick.AddListener(OnOpenSettingsButtonClicked);
            openPauseButton.onClick.AddListener(OnOpenPauseButtonClicked);
            openQuitButton.onClick.AddListener(OnOpenQuitButtonClicked);

            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            pauseQuitButton.onClick.AddListener(OnOpenQuitButtonClicked);
            
            backButton.onClick.AddListener(OnBackButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            
            againButton.onClick.AddListener(OnResetButtonClicked);
            nextStageButton.onClick.AddListener(OnQuitButtonClicked);
            
            restartButton.onClick.AddListener(OnResetButtonClicked);
            homeButton.onClick.AddListener(OnQuitButtonClicked);
        }

        
        void OnOpenSettingsButtonClicked()
        {
            if (!settingsWindow.gameObject.activeSelf)
            {
                settingsWindow.gameObject.SetActive(true);
            }
        }
        
        void OnOpenPauseButtonClicked()
        {
            pauseWindow.gameObject.SetActive(true);
            
            AudioSetting.MasterVolume = 0;
        }
        void OnOpenQuitButtonClicked()
        {
            quitWindow.gameObject.SetActive(true);
        }
        
        void OnResetButtonClicked()
        {
            Loading.LoadScene("InGame");
        }


        void OnResumeButtonClicked()
        {
            pauseWindow.gameObject.SetActive(false);

            AudioSetting.MasterVolume = volumeSlider.value;
        }


        void OnBackButtonClicked()
        {
            WindowAnimation.CloseWindow(quitWindow);
        }
        void OnQuitButtonClicked()
        {
            Loading.LoadScene("Lobby");
        }
    }
}
