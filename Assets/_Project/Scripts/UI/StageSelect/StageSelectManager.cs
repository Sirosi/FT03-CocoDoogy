using CocoDoogy.Core;
using CocoDoogy.Data;
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
    public class StageSelectManager : Singleton<StageSelectManager>
    {
        public static int SelectedStage;
        
        
        [Header("Main UIs")]
        [SerializeField] private RectTransform lobbyUIPanel;
        [SerializeField] private RectTransform stageSelectUIPanel;
        
        [Header("UI Elements")]
        [SerializeField] private StageSwap stageSwap;
        [SerializeField] private StageInfoPanel stageInfoPanel;
        
        [Header("Menu Buttons")]
        [SerializeField] private CommonButton backButton;
        
        [Header("Stages")]
        [SerializeField] private Sprite lockedSprite;
        
        [Header("StageOptions")]
        [SerializeField] private int clearedStages;


        private Theme nowTheme = Theme.None;
        private bool isStageSelect;
        private bool isStageReady;
        private Image[] stageIcons;
        
        
        protected override void Awake()
        {
            base.Awake();

            PageCameraSwiper.OnStartPageChanged += OnChangedTheme;
            
            stageInfoPanel.gameObject.SetActive(false);
            isStageSelect = true;
            isStageReady = false;
            
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            PageCameraSwiper.OnStartPageChanged -= OnChangedTheme;
        }


        private void OnChangedTheme(Theme theme)
        {
            nowTheme = theme;
        }
        
        //private void OnStageButtonClicked(int index)
        //{
        //    if (index <= clearedStages)
        //    {
        //        SelectedStage = index;
        //        
        //        if (stageReadyUI.gameObject.activeSelf) return;
        //        stageReadyUI.gameObject.SetActive(true);
        //        isStageSelect = false;
        //        isStageReady = true;
        //    }
        //    else
        //    {
        //        MessageDialog.ShowMessage($"STAGE {index}","이전 스테이지를 클리어해주세요!", DialogMode.Confirm, (_) => Debug.Log("Refused"));
        //    }
        //}

        
        
        private void OnBackButtonClicked()
        {
            if (isStageSelect)
            {
                WindowAnimation.SwipeWindow(stageSelectUIPanel);
                lobbyUIPanel.gameObject.SetActive(true);
            }
            if (isStageReady)
            {
                WindowAnimation.SwipeWindow(stageInfoPanel.transform as RectTransform);
                
                isStageSelect = true;
                isStageReady = false;
            }
        }


        public static void ShowReadyView(StageData data)
        {
            if (!Instance) return;
            
            Instance.stageInfoPanel.Show(data);
        }
    }
}
