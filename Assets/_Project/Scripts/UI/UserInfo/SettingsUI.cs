using CocoDoogy.Audio;
using CocoDoogy.CameraSwiper;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI.InGame;
using DG.Tweening;
using System.Collections.Generic;
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
        
        private bool _isMasterMuted;
        private bool _isBgmMuted;
        private bool _isSfxMuted;
        
        private bool _isUpdatingMuteState = false;
        
        private const float Color_Disabled = 0.5f;
        private const float Color_Enabled = 1.0f;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
            
            masterVolume.onValueChanged.AddListener(MasterControl);
            bgmVolume.onValueChanged.AddListener(BGMControl);
            sfxVolume.onValueChanged.AddListener(SfxControl);

            masterMute.onClick.AddListener(()=> MasterMute(!_isMasterMuted));
            bgmMute.onClick.AddListener(()=> BgmMute(!_isBgmMuted));
            sfxMute.onClick.AddListener(()=> SfxMute(!_isSfxMuted));
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
            // 재귀 호출 차단
            if (_isUpdatingMuteState) return;
            
            _isUpdatingMuteState = true;
            
            // Master 상태 변경
            SetMasterMuteInternal(mute);
            
            // Master가 꺼지면 하위도 모두 끄기, 켜지면 하위도 모두 켜기
            SetBgmMuteInternal(mute);
            SetSfxMuteInternal(mute);
            
            UpdateMasterVisual();
            _isUpdatingMuteState = false;
        }

        private void BgmMute(bool mute)
        {
            // 재귀 호출 차단
            if (_isUpdatingMuteState) return;
            
            _isUpdatingMuteState = true;
            
            // Bgm이 켜지는데 Master가 꺼져있으면 Master도 켜기
            if (!mute && _isMasterMuted)
            {
                SetMasterMuteInternal(false);
            }
            
            SetBgmMuteInternal(mute);
            
            // Bgm과 Sfx가 둘 다 꺼지면 Master도 끄기
            if (mute && _isSfxMuted)
            {
                SetMasterMuteInternal(true);
            }
            
            UpdateMasterVisual();
            _isUpdatingMuteState = false;
        }

        private void SfxMute(bool mute)
        {
            // 재귀 호출 차단
            if (_isUpdatingMuteState) return;
            
            _isUpdatingMuteState = true;
            
            // Sfx가 켜지는데 Master가 꺼져있으면 Master도 켜기
            if (!mute && _isMasterMuted)
            {
                SetMasterMuteInternal(false);
            }
            
            SetSfxMuteInternal(mute);
            
            // Bgm과 Sfx가 둘 다 꺼지면 Master도 끄기
            if (mute && _isBgmMuted)
            {
                SetMasterMuteInternal(true);
            }
            
            UpdateMasterVisual();
            _isUpdatingMuteState = false;
        }
        #endregion

        #region Internal Mute Setters (재귀 없이 상태만 변경)
        private void SetMasterMuteInternal(bool mute)
        {
            _isMasterMuted = mute;
            VolumeController.Instance.SetMasterVolume(mute ? 0 : masterVolume.value);
        }
        
        private void SetBgmMuteInternal(bool mute)
        {
            _isBgmMuted = mute;
            VolumeController.Instance.SetBgmVolume(mute ? 0 : bgmVolume.value);
        }
        
        private void SetSfxMuteInternal(bool mute)
        {
            _isSfxMuted = mute;
            VolumeController.Instance.SetSfxVolume(mute ? 0 : sfxVolume.value);
        }
        #endregion

        #region 비주얼 업데이트
        private void UpdateMasterVisual()
        {
            // 1. Master 자기 자신 색상 변경
            float targetValue = (_isMasterMuted || masterVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, masterIcon, new List<Image> { masterSlider, masterVolume.image });

            // 2. Master 상태가 변하면 하위 UI들도 Master의 영향을 받으므로 연쇄 호출
            UpdateBgmVisual();
            UpdateSfxVisual();
        }
        
        private void UpdateBgmVisual()
        {
            // Master가 어두워야 하거나 본인이 어두워야 할 때 0.5f 적용
            float targetValue = (_isMasterMuted || _isBgmMuted || bgmVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, bgmIcon, new List<Image> { bgmSlider, bgmVolume.image });
        }
        
        private void UpdateSfxVisual()
        {
            float targetValue = (_isMasterMuted || _isSfxMuted || sfxVolume.value <= 0) ? Color_Disabled : Color_Enabled;
            ApplyColorToGroup(targetValue, sfxIcon, new List<Image> { sfxSlider, sfxVolume.image, sfxInnerIcon });
        }
        
        //Dotween은 여기서 사용
        private void ApplyColorToGroup(float alphaValue, Image icon, List<Image> images)
        {
            Color targetColor = new Color(alphaValue, alphaValue, alphaValue, 1f);
            float duration = 0.2f;

            if (icon != null) icon.DOColor(targetColor, duration).SetUpdate(true);
            
            foreach (var img in images)
            {
                if (img != null) img.DOColor(targetColor, duration).SetUpdate(true);
            }
        }
        #endregion
    }
}
