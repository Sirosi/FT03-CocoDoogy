using CocoDoogy.Audio;
using CocoDoogy.CameraSwiper;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI.InGame;
using DG.Tweening;
using System.Diagnostics;
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
        [SerializeField] private Image sfxInnerIcon;
        
        [Header("Volume UI Sliders")]
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;

        [Header("Mute")]
        [SerializeField] private Button masterMute;
        [SerializeField] private Button bgmMute;
        [SerializeField] private Button sfxMute;
        private float lastMasterVolume;
        private float lastBgmVolume;
        private float lastSfxVolume;
        


        void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            
            masterVolume.onValueChanged.AddListener(MasterControl);
            bgmVolume.onValueChanged.AddListener(BGMControl);
            sfxVolume.onValueChanged.AddListener(SfxControl);

            masterMute.onClick.AddListener(()=> MasterMute(true));
            bgmMute.onClick.AddListener(()=> BgmMute(true));
            sfxMute.onClick.AddListener(()=> SfxMute(true));
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
            Time.timeScale = 0;
            InGameManager.Timer.Pause();
        }

        void OnDisable()
        {
            InGameManager.Timer.Start();
            Time.timeScale = 1;
        }
        
        public override void ClosePanel()
        {
            settingsWindow.gameObject.SetActive(false);
            PageCameraSwiper.IsSwipeable = true;
        }

        void MasterControl(float value)
        {
            if (value <= 0)
            {
                masterIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
            }
            else
            {
                masterIcon.DOColor(new Color(1, 1, 1), 0.2f);
                MasterMute(false);
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
                BgmMute(false);
            }

            AudioSetting.BgmVolume = value;
            
        }
        void SfxControl(float value)
        {
            if (value <= 0)
            {
                sfxIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
                sfxInnerIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f);
            }
            else
            {
                sfxIcon.DOColor(new Color(1, 1, 1), 0.2f);
                sfxInnerIcon.DOColor(new Color(1, 1, 1), 0.2f);
                SfxMute(false);
            }

            AudioSetting.SfxVolume = value;
            
        }

        #region Mute Events
        void MasterMute(bool mute)
        {
            masterMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                masterMute.onClick.AddListener(()=> MasterMute(false));
                lastMasterVolume = masterVolume.value;
                masterVolume.value = 0;
            }
            else
            {
                masterMute.onClick.AddListener(()=> MasterMute(true));
                if (masterVolume.value <= 0) masterVolume.value = lastMasterVolume;
                AudioSetting.MasterVolume = masterVolume.value;
            }
        }
        void BgmMute(bool mute)
        {
            bgmMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                bgmMute.onClick.AddListener(()=> BgmMute(false));
                lastBgmVolume = bgmVolume.value;
                bgmVolume.value = 0;
            }
            else
            {
                bgmMute.onClick.AddListener(()=> BgmMute(true));
                if (bgmVolume.value <= 0) bgmVolume.value = lastBgmVolume;
                AudioSetting.BgmVolume = bgmVolume.value;
            }
        }
        void SfxMute(bool mute)
        {
            sfxMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                sfxMute.onClick.AddListener(()=> SfxMute(false));
                lastSfxVolume = sfxVolume.value;
                sfxVolume.value = 0;
            }
            else
            {
                sfxMute.onClick.AddListener(()=> SfxMute(true));
                if (sfxVolume.value <= 0) sfxVolume.value = lastSfxVolume;
                AudioSetting.SfxVolume = sfxVolume.value;
            }
        }
        #endregion
    }
}
