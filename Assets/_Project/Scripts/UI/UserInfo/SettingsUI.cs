using CocoDoogy.Audio;
using CocoDoogy.CameraSwiper;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI.InGame;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace CocoDoogy.UI.UserInfo
{
    public class SettingsUI : UIPanel
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform settingsWindow;
        
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        
        [Header("Volume Icons")]
        [SerializeField] private Image masterIcon;
        [SerializeField] private Image masterInnerIcon;
        [SerializeField] private Image bgmIcon;
        [SerializeField] private Image sfxIcon;
        
        [Header("Volume UI Sliders")]
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;

        [Header("Mute")]
        [SerializeField] private Toggle masterMute;
        [SerializeField] private Toggle bgmMute;
        [SerializeField] private Toggle sfxMute;
        private float lastMasterVolume;
        private float lastBgmVolume;
        private float lastSfxVolume;
        


        void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            
            masterVolume.onValueChanged.AddListener(MasterControl);
            bgmVolume.onValueChanged.AddListener(BGMControl);
            sfxVolume.onValueChanged.AddListener(SfxControl);

            masterMute.onValueChanged.AddListener(MasterMute);
            bgmMute.onValueChanged.AddListener(BgmMute);
            sfxMute.onValueChanged.AddListener(SfxMute);
        }
        void Start()
        {
            masterVolume.value = AudioSetting.MasterVolume;
            bgmVolume.value = AudioSetting.BgmVolume;
            sfxVolume.value = AudioSetting.SfxVolume;
            
            MasterControl(masterVolume.value);
            BGMControl(bgmVolume.value);
            SfxControl(sfxVolume.value);
        }

        void OnEnable()
        {
            InGameManager.Timer.Pause();
        }

        void OnDisable()
        {
            InGameManager.Timer.Start();
        }
        
        public override void ClosePanel()
        {
            WindowAnimation.SwipeWindow(settingsWindow);
            PageCameraSwiper.IsSwipeable = true;
        }

        void MasterControl(float value)
        {
            if (value <= 0)
            {
                masterIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
                masterInnerIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
            }
            else
            {
                masterIcon.DOColor(new Color(1, 1, 1), 0.2f);
                masterInnerIcon.DOColor(new Color(1, 1, 1), 0.2f);
                masterMute.isOn = false;
            }

            AudioSetting.MasterVolume = value;

        }
        void BGMControl(float value)
        {
            if (value <= 0)
            {
                bgmIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
            }
            else
            {
                bgmIcon.DOColor(new Color(1, 1, 1), 0.2f);
                bgmMute.isOn = false;
            }

            AudioSetting.BgmVolume = value;
            
        }
        void SfxControl(float value)
        {
            if (value <= 0)
            {
                sfxIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
            }
            else
            {
                sfxIcon.DOColor(new Color(1, 1, 1), 0.2f);
                sfxMute.isOn = false;
            }

            AudioSetting.SfxVolume = value;
            
        }

        #region Mute Events
        void MasterMute(bool isOn)
        {
            if (isOn)
            {
                lastMasterVolume = masterVolume.value;
                masterVolume.value = 0;
            }
            else
            {
                if (masterVolume.value <= 0) masterVolume.value = lastMasterVolume;
                AudioSetting.MasterVolume = masterVolume.value;
            }
        }
        void BgmMute(bool isOn)
        {
            if (isOn)
            {
                lastBgmVolume = bgmVolume.value;
                bgmVolume.value = 0;
            }
            else
            {
                if (bgmVolume.value <= 0) bgmVolume.value = lastBgmVolume;
                AudioSetting.BgmVolume = bgmVolume.value;
            }
        }
        void SfxMute(bool isOn)
        {
            if (isOn)
            {
                lastSfxVolume = sfxVolume.value;
                sfxVolume.value = 0;
            }
            else
            {
                if (sfxVolume.value <= 0) sfxVolume.value = lastSfxVolume;
                AudioSetting.SfxVolume = sfxVolume.value;
            }
        }
        #endregion
    }
}
