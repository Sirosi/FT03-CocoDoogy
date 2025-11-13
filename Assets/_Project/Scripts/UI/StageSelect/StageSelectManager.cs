using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSelectManager : MonoBehaviour
    {
        public static StageSelectManager Instance { get; private set; }
        
        [Header("UI Elements")]
        [SerializeField] private RectTransform stageSelectUI;
        [SerializeField] private RectTransform stageReadyUI;
        private bool isStageSelect;
        private bool isStageReady;
        
        [Header("Menu Buttons")]
        [SerializeField] private CommonButton backButton;
        
        [Header("Stages")]
        [SerializeField] private GameObject[] stages;
        [SerializeField] private Sprite lockedSprite;
        private CommonButton[] stageButtons;
        private Image[] stageIcons;
        
        [Header("StageOptions")]
        [SerializeField] private int clearedStages;
        public static int SelectedStage;
            
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            isStageSelect = true;
            isStageReady = false;
            
            
            
            stageButtons = new CommonButton[stages.Length];
            stageIcons = new Image[stages.Length];
            for (int i = 0; i < stages.Length; ++i)
            {
                stageButtons[i] = stages[i].GetComponent<CommonButton>();
                stageIcons[i] = stages[i].GetComponent<Image>();
                
                LockStage(i);
                int index = i + 1;
                stageButtons[i].onClick.AddListener(()=> OnStageButtonClicked(index));
            }
            
            
            
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void LockStage(int index)
        {
            if (index >= clearedStages)
            {
                stageIcons[index].sprite = lockedSprite;
                foreach (Transform child in stages[index].transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        
        private void OnStageButtonClicked(int index)
        {
            if (index <= clearedStages)
            {
                SelectedStage = index;
                
                if (stageReadyUI.gameObject.activeSelf) return;
                stageReadyUI.gameObject.SetActive(true);
                isStageSelect = false;
                isStageReady = true;
            }
            else
            {
                MessageDialog.ShowMessage($"STAGE {index}","이전 스테이지를 클리어해주세요!", DialogMode.Confirm, (_) => Debug.Log("Refused"));
            }
        }

        
        
        private void OnBackButtonClicked()
        {
            if (isStageSelect) Loading.LoadScene("MainUITest");
            if (isStageReady)
            {
                WindowAnimation.SwipeWindow(stageReadyUI);
                
                isStageSelect = true;
                isStageReady = false;
            }
        }
    }
}
