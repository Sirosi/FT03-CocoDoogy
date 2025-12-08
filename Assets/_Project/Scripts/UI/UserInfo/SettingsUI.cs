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
        [SerializeField] private Image masterSlider;
        [SerializeField] private Image bgmSlider;
        [SerializeField] private Image sfxSlider;
        


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
                MasterMute(true);
            }
            else
            {
                MasterMute(false);
            }

            AudioSetting.MasterVolume = value;

        }
        private void BGMControl(float value)
        {
            if (value <= 0)
            {
                BgmMute(true);
            }
            else
            {
                BgmMute(false);
            }

            AudioSetting.BgmVolume = value;
            
        }
        private void SfxControl(float value)
        {
            if (value <= 0)
            {
                SfxMute(true);
            }
            else
            {
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
                VolumeController.Instance.SetMasterVolume(0);
                
                masterIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
                masterSlider.color = new Color(0.5f, 0.5f, 0.5f);
                masterVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                masterMute.onClick.AddListener(()=> MasterMute(true));
                VolumeController.Instance.SetMasterVolume(masterVolume.value);
                
                masterIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                masterSlider.color = new Color(1, 1, 1);
                masterVolume.image.color = new Color(1, 1, 1);
            }
        }
        private void BgmMute(bool mute)
        {
            bgmMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                bgmMute.onClick.AddListener(()=> BgmMute(false));
                VolumeController.Instance.SetBgmVolume(0);
                
                bgmIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
                bgmSlider.color = new Color(0.5f, 0.5f, 0.5f);
                bgmVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                bgmMute.onClick.AddListener(()=> BgmMute(true));
                VolumeController.Instance.SetMasterVolume(bgmVolume.value);
                
                bgmIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                bgmSlider.color = new Color(1, 1, 1);
                bgmVolume.image.color = new Color(1, 1, 1);
            }
        }
        private void SfxMute(bool mute)
        {
            sfxMute.onClick.RemoveAllListeners();
            
            if (mute)
            {
                sfxMute.onClick.AddListener(()=> SfxMute(false));
                VolumeController.Instance.SetBgmVolume(0);
                
                sfxIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
                sfxInnerIcon.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f).SetUpdate(true);
                sfxSlider.color = new Color(0.5f, 0.5f, 0.5f);
                sfxVolume.image.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                sfxMute.onClick.AddListener(()=> SfxMute(true));
                VolumeController.Instance.SetMasterVolume(sfxVolume.value);
                
                sfxIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                sfxInnerIcon.DOColor(new Color(1, 1, 1), 0.2f).SetUpdate(true);
                sfxSlider.color = new Color(1, 1, 1);
                sfxVolume.image.color = new Color(1, 1, 1);
            }
        }
        #endregion
    }
}
