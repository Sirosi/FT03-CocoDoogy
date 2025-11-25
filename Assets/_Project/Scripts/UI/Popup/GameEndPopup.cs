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
        
        private Action<CallbackType> callback = null;
        
        private void Awake()
        {
            if (gameEndPopup == null)
            {
                gameEndPopup = this;
            }
            
            gameObject.SetActive(false);
            restartButton.onClick.AddListener(OnClickRestart);
            homeButton.onClick.AddListener(OnClickHome);
            nextButton.onClick.AddListener(OnClickNext);
        }

        public static void OpenPopup(bool isDefeat)
        {
            gameEndPopup.gameObject.SetActive(true);

            gameEndPopup.titleImage.sprite = !isDefeat ? gameEndPopup.completeUI.titleSprite : gameEndPopup.defeatUI.titleSprite;
            gameEndPopup.titleTextImage.sprite = !isDefeat ? gameEndPopup.completeUI.titleTextSprite : gameEndPopup.defeatUI.titleTextSprite;
            
            gameEndPopup.clearEffectImage.sprite = !isDefeat ? gameEndPopup.completeUI.effectBackground : gameEndPopup.defeatUI.effectBackground;
            
            gameEndPopup.complete.SetActive(!isDefeat);
            gameEndPopup.defeat.SetActive(isDefeat);
            
            gameEndPopup.remainAPText.text = "추가";
            gameEndPopup.clearTimeText.text = "추가";
        }
        
        private void OnClickRestart()
        {
            InGameUIManager.Instance.OnResetButtonClicked();
        }
        private void OnClickHome()
        {
            InGameUIManager.Instance.OnQuitButtonClicked();
        }
        private void OnClickNext()
        {
           // 현재 테마의 최대 스테이지를 가져옴
           var stage = DataManager.GetStageData(InGameManager.Stage.theme).Count;
           var theme = InGameManager.Stage.theme;
           
           // 가져온 최대 스테이지와 현재 플레이한 스테이지의 인덱스를 비교
           if (InGameManager.Stage.index + 1 < stage) 
           {
               // 플레이한 스테이지에 1을 더한값 보다 최대 스테이지가 크면 다음 스테이지로 넘어감.
               Debug.Log("다음 스테이지로 이동합니다.");
           }
           else if (InGameManager.Stage.index + 1 >= stage && Enum.IsDefined(typeof(Theme), (int)theme << 1)) 
           {
               // 플레이한 스테이지에 1을 더한값 보다 최대 스테이지가 작거나 같고 다음 테마가 존재한다면 다음 테마의 1스테이로 넘어감.
               Debug.Log("다음 테마의 1스테이지로 넘어갑니다.");
           }
           else if (InGameManager.Stage.index + 1 >= stage && !Enum.IsDefined(typeof(Theme), (int)theme << 1))
           {
               // 플레이한 스테이지에 1을 더한값 보다 최대 스테이지가 작거나 같고 다음 테마가 존재하지 않는다면 버튼을 비활성화.
               Debug.Log("다음 테마가 존재하지 않습니다.");
               nextButton.interactable = false;
           }
        }
    }
}