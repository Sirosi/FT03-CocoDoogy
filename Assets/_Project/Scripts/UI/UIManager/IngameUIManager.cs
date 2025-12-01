using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.Network;
using CocoDoogy.UI.InGame;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.UserInfo;
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

        [Header("Option UI Elements")]
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private InGamePauseUI pauseUI;

        protected override void Awake()
        {
            base.Awake();
            openSettingsButton.onClick.AddListener(OnClickSetting);
            openPauseButton.onClick.AddListener(OnClickPause);
        }
        
        private void OnClickSetting()
        {
            settingsUI.OpenPanel();
        }

        private void OnClickPause()
        {
            pauseUI.OpenUI();
            SfxManager.PlayDucking(0.7f);
        }
    }
}
