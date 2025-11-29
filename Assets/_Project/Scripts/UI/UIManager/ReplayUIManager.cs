using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.UI.Replay;
using CocoDoogy.UI.UserInfo;
using UnityEngine;

namespace CocoDoogy.UI.UIManager
{
    public class ReplayUIManager : Singleton<ReplayUIManager>
    {
        [Header("Buttons")]
        [SerializeField] private CommonButton openSettingsButton;
        [SerializeField] private CommonButton openPauseButton;
        [SerializeField] private CommonButton redoButton;
        [SerializeField] private CommonButton undoButton;
        
        [Header("UI Panel")]
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private ReplayPauseUI replayPauseUI;
        
        protected override void Awake()
        {
            base.Awake();
            openSettingsButton.onClick.AddListener(OnClickOpenSetting);
            openPauseButton.onClick.AddListener(OnClickOpenPause);
            
            redoButton.onClick.AddListener(Redo);
            undoButton.onClick.AddListener(Undo);
        }

        private void OnClickOpenSetting()
        {
            SfxManager.PlayDucking(0.7f);
            settingsUI.OpenPanel();
        }

        private void OnClickOpenPause()
        {
            replayPauseUI.OpenUI();
        }
        
        private void Undo()
        {
            CommandManager.UndoCommandAuto();
        }
        private void Redo()
        {
            CommandManager.RedoCommandAuto();
        }
    }
}