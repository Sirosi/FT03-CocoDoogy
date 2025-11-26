using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI.UIManager;
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

        [Header("Score Elements")] 
        [SerializeField] private GameObject complete;
        [SerializeField] private GameObject defeat;
        
        [Header("Clear Effect Element")]
        [SerializeField] private Image clearEffectImage;
        
        [Header("Info Elements")]
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

        public static void OpenPopup(bool isDefeat)
        {
            gameEndPopup.panel.SetActive(true);

            gameEndPopup.titleImage.sprite = !isDefeat ? gameEndPopup.completeUI.titleSprite : gameEndPopup.defeatUI.titleSprite;
            gameEndPopup.titleTextImage.sprite = !isDefeat ? gameEndPopup.completeUI.titleTextSprite : gameEndPopup.defeatUI.titleTextSprite;
            
            gameEndPopup.clearEffectImage.sprite = !isDefeat ? gameEndPopup.completeUI.effectBackground : gameEndPopup.defeatUI.effectBackground;
            
            gameEndPopup.complete.SetActive(!isDefeat);
            gameEndPopup.defeat.SetActive(isDefeat);
            
            gameEndPopup.remainAPText.text = $"{InGameManager.RefillPoints * InGameManager.CurrentMapMaxActionPoints + InGameManager.ActionPoints}";
            
            OnTimeChanged(InGameManager.Timer.NowTime);
            
            if (DataManager.GetStageData(InGameManager.Stage.theme, InGameManager.Stage.index + 1))
            {
                gameEndPopup.nextButton.interactable = true;    
                gameEndPopup.enableNextButton.SetActive(true);
                gameEndPopup.disableNextButton.SetActive(false);
                return;
            }
            
            // 다음 스테이지가 없다면 NextButton을 비활성화
            gameEndPopup.nextButton.interactable = false;
            gameEndPopup.enableNextButton.SetActive(false);
            gameEndPopup.disableNextButton.SetActive(true);
        }
        private static void OnTimeChanged(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            gameEndPopup.clearTimeText.text = $"{minutes:00}:{seconds:00}";
        }
        private void OnClickRestart()
        {
            ItemHandler.UseItem();
            InGameUIManager.Instance.OnResetButtonClicked();
        }
        private void OnClickHome()
        {
            ItemHandler.UseItem();
            InGameUIManager.Instance.OnQuitButtonClicked();
        }
        private void OnClickNext()
        {
            ItemHandler.UseItem();
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
               InGameUIManager.Instance.OnResetButtonClicked();
           }
        }
    }
}