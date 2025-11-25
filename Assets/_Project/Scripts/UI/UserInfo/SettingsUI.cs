using CocoDoogy.Timer;
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
        [SerializeField] private Image bgmIcon;
        [SerializeField] private Image sfxIcon;
        [SerializeField] private Sprite onMasterIcon;
        [SerializeField] private Sprite onBgmIcon;
        [SerializeField] private Sprite onSfxIcon;
        [SerializeField] private Sprite offMasterIcon;
        [SerializeField] private Sprite offBgmIcon;
        [SerializeField] private Sprite offSfxIcon;
        
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
        }

        void OnEnable()
        {
            InGameTimer.ToggleTimer();
        }

        void OnDisable()
        {
            InGameTimer.ToggleTimer();
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
                masterIcon.sprite = offMasterIcon;
            }
            else
            {
                masterIcon.sprite = onMasterIcon;
                masterMute.isOn = false;
            }

            AudioSetting.MasterVolume = value;

        }
        void BGMControl(float value)
        {
            if (value <= 0)
            {
                bgmIcon.sprite = offBgmIcon;
            }
            else
            {
                bgmIcon.sprite = onBgmIcon;
                bgmMute.isOn = false;
            }

            AudioSetting.BgmVolume = value;
            
        }
        void SfxControl(float value)
        {
            if (value <= 0)
            {
                sfxIcon.sprite = offSfxIcon;
            }
            else
            {
                sfxIcon.sprite = onSfxIcon;
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
