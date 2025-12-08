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
        


        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            
            masterVolume.onValueChanged.AddListener(MasterControl);
            bgmVolume.onValueChanged.AddListener(BGMControl);
            sfxVolume.onValueChanged.AddListener(SfxControl);

            masterMute.onClick.AddListener(()=> MasterMute(true));
            bgmMute.onClick.AddListener(()=> BgmMute(true));
            sfxMute.onClick.AddListener(()=> SfxMute(true));
        }
        private void Start()
        {
            masterVolume.value = AudioSetting.MasterVolume;
            bgmVolume.value = AudioSetting.BgmVolume;
            sfxVolume.value = AudioSetting.SfxVolume;
            
            MasterControl(masterVolume.value);
            BGMControl(bgmVolume.value);
            SfxControl(sfxVolume.value);
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
            InGameManager.Timer.Pause();
        }

        private void OnDisable()
        {
            InGameManager.Timer.Start();
            Time.timeScale = 1;
        }
        
        public override void ClosePanel()
        {
            settingsWindow.gameObject.SetActive(false);
            PageCameraSwiper.IsSwipeable = true;
        }

        private void MasterControl(float value)
        {
            if (value <= 0)
            {
                masterIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
            }
            else
            {
                masterIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                MasterMute(false);
            }

            AudioSetting.MasterVolume = value;

        }
        private void BGMControl(float value)
        {
            if (value <= 0)
            {
                bgmIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
            }
            else
            {
                bgmIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                BgmMute(false);
            }

            AudioSetting.BgmVolume = value;
            
        }
        private void SfxControl(float value)
        {
            if (value <= 0)
            {
                sfxIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
                sfxInnerIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
            }
            else
            {
                sfxIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                sfxInnerIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                SfxMute(false);
            }

            AudioSetting.SfxVolume = value;
            
        }

        #region Mute Events
        private void MasterMute(bool mute)
        {
            masterMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                masterMute.onClick.AddListener(()=> MasterMute(false));
                lastMasterVolume = masterVolume.value;
                masterVolume.value = 0;
                
                masterVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                masterMute.onClick.AddListener(()=> MasterMute(true));
                if (masterVolume.value <= 0) masterVolume.value = lastMasterVolume;
                AudioSetting.MasterVolume = masterVolume.value;
                
                masterVolume.image.color = new Color(1, 1, 1);
            }
        }
        private void BgmMute(bool mute)
        {
            bgmMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                bgmMute.onClick.AddListener(()=> BgmMute(false));
                lastBgmVolume = bgmVolume.value;
                bgmVolume.value = 0;
                
                bgmVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                bgmMute.onClick.AddListener(()=> BgmMute(true));
                if (bgmVolume.value <= 0) bgmVolume.value = lastBgmVolume;
                AudioSetting.BgmVolume = bgmVolume.value;
                
                bgmVolume.image.color = new Color(1, 1, 1);
            }
        }
        private void SfxMute(bool mute)
        {
            sfxMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                sfxMute.onClick.AddListener(()=> SfxMute(false));
                lastSfxVolume = sfxVolume.value;
                sfxVolume.value = 0;
                
                sfxVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                sfxMute.onClick.AddListener(()=> SfxMute(true));
                if (sfxVolume.value <= 0) sfxVolume.value = lastSfxVolume;
                AudioSetting.SfxVolume = sfxVolume.value;
                
                sfxVolume.image.color = new Color(1, 1, 1);
            }
        }
        #endregion
    }
}
