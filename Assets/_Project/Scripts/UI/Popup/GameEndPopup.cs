using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Network;
using CocoDoogy.UI.UIManager;
using CocoDoogy.Utility.Loading;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Popup
{
    public class GameEndPopup : MonoBehaviour
    {
        private static GameEndPopup gameEndPopup;
        
        [Header("UI Panel")]
        [SerializeField] private GameObject panel;
        
        [Header("Complete UI Elements")]
        [SerializeField] private GameEndWindow completeUI;
        
        [Space(10)]
        
        [Header("Defeat UI Elements")]
        [SerializeField] private GameEndWindow defeatUI;
        
        
        [Header("Title Elements")]
        [SerializeField] private Image titleImage;
        [SerializeField] private Image titleTextImage;
        [SerializeField] private Image backgroundStarImage;

        [Header("Score Elements")] 
        [SerializeField] private CompleteScore completeScore;
        [SerializeField] private GameObject defeatScore;
        
        [Header("Clear Effect Element")]
        [SerializeField] private Image clearEffectImage;
        
        [Header("Info Elements")]
        [SerializeField] private TextMeshProUGUI remainResetText;
        [SerializeField] private TextMeshProUGUI remainAPText;
        [SerializeField] private TextMeshProUGUI clearTimeText;
        
        [Header("Button Elements")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button nextButton;
        
        [Header("Button State")]
        [SerializeField] private GameObject enableNextButton;
        [SerializeField] private GameObject disableNextButton;
        
        private Action<CallbackType> callback = null;
        
        private void Awake()
        {
            if (gameEndPopup == null)
            {
                gameEndPopup = this;
            }
            
            panel.SetActive(false);
            restartButton.onClick.AddListener(OnClickRestart);
            homeButton.onClick.AddListener(OnClickHome);
            nextButton.onClick.AddListener(OnClickNext);
        }

        public static void OpenPopup(bool isDefeat, int star)
        {
            gameEndPopup.panel.SetActive(true);

            gameEndPopup.titleImage.sprite =
                !isDefeat ? gameEndPopup.completeUI.titleSprite : gameEndPopup.defeatUI.titleSprite;
            gameEndPopup.titleTextImage.sprite =
                !isDefeat ? gameEndPopup.completeUI.titleTextSprite : gameEndPopup.defeatUI.titleTextSprite;
            gameEndPopup.clearEffectImage.sprite = !isDefeat
                ? gameEndPopup.completeUI.effectBackground
                : gameEndPopup.defeatUI.effectBackground;
            
            gameEndPopup.completeScore.gameObject.SetActive(!isDefeat);
            gameEndPopup.defeatScore.SetActive(isDefeat);
            
            gameEndPopup.backgroundStarImage.sprite =
                !isDefeat ? gameEndPopup.completeUI.effectText : gameEndPopup.defeatUI.effectText;

            gameEndPopup.remainResetText.text = PlayerHandler.IsReplay ?
                $"{InGameManager.UseRefillCounts}" : $"{ReplayUIManager.refillCount}";
            
            gameEndPopup.remainAPText.text = PlayerHandler.IsReplay ?
                $"{InGameManager.UseActionPoints}" : $"{ReplayUIManager.consumeAP}";
            OnTimeChanged(!PlayerHandler.IsReplay ? InGameManager.Timer.NowTime : (float)ReplayUIManager.timer);
            gameEndPopup.completeScore.GetStageClearResult(isDefeat, star);
            
            if (isDefeat || !DataManager.GetStageData(InGameManager.Stage.theme, InGameManager.Stage.index + 1) || PlayerHandler.IsReplay)
            {
                // 다음 스테이지가 없다면 NextButton을 비활성화
                gameEndPopup.nextButton.interactable = false;
                gameEndPopup.enableNextButton.SetActive(false);
                gameEndPopup.disableNextButton.SetActive(true);
                return;
            }
            
            gameEndPopup.nextButton.interactable = true;
            gameEndPopup.enableNextButton.SetActive(true);
            gameEndPopup.disableNextButton.SetActive(false);
        }

        private static void OnTimeChanged(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            gameEndPopup.clearTimeText.text = $"{minutes:00}:{seconds:00}";
        }
        private void OnClickRestart()
        {
            Restart();
        }
        private void OnClickHome()
        {
            SfxManager.StopDucking();
            Loading.LoadScene("Lobby");
        }
        private void OnClickNext()
        {
           // 현재 테마의 최대 스테이지를 가져옴
           var theme = InGameManager.Stage.theme;
           var index = InGameManager.Stage.index;

           StageData nextStage = DataManager.GetStageData(theme, index + 1);
           
           // 현재 테마의 다음 스테이지가 존재한다면
           if (nextStage && nextStage.theme == theme) 
           {
               Debug.Log($"theme : {nextStage.theme}, index : {nextStage.index}, stageName : {nextStage.stageName}");
               Debug.Log("다음 스테이지로 이동합니다.");
               
               InGameManager.Stage = nextStage;
               Restart();
           }
        }

        private async void Restart()
        {
            bool isReady = await FirebaseManager.UseTicketAsync();
            if (isReady)
            {
                SfxManager.StopDucking();
                Loading.LoadScene(PlayerHandler.IsReplay ? "Replay" : "InGame");
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
    }
}