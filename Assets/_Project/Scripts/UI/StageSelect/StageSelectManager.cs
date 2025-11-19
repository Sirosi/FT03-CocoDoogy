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
        [Header("Main UIs")]
        [SerializeField] private RectTransform lobbyUIPanel;
        [SerializeField] private RectTransform stageSelectUIPanel;
        
        
        [Header("UI Elements")]
        [SerializeField] private StageSwap stageSwap;
        [SerializeField] private StageInfoPanel stageInfoPanel;
        private bool isStageSelect;
        private bool isStageReady;
        
        [Header("Menu Buttons")]
        [SerializeField] private CommonButton backButton;
        
        [Header("Stages")]
        [SerializeField] private Sprite lockedSprite;
        private Image[] stageIcons;
        
        [Header("StageOptions")]
        [SerializeField] private int clearedStages;
        public static int SelectedStage;
            
        protected override void Awake()
        {
            base.Awake();
            
            stageInfoPanel.gameObject.SetActive(false);
            isStageSelect = true;
            isStageReady = false;
            
            backButton.onClick.AddListener(OnBackButtonClicked);
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
