using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System;
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
        /// <summary>
        /// 해당 계정이 가장 마지막에 클리어한 가장 높은 스테이지
        /// </summary>
        public static StageInfo LastClearedStage { get; set; }

        [Header("Main UIs")]
        [SerializeField] private RectTransform lobbyUIPanel;
        [SerializeField] private RectTransform stageSelectUIPanel;

        [Header("UI Elements")]
        [SerializeField] private StageListPage stageListPage;
        [SerializeField] private StageInfoPanel stageInfoPanel;

        [Header("Menu Buttons")]
        [SerializeField] private CommonButton backButton;

        [Header("Stages")]
        [SerializeField] private Sprite lockedSprite;

        [Header("StageOptions")]
        [SerializeField] private int clearedStages;


        private Theme nowTheme = Theme.None;
        private Image[] stageIcons;


        protected override void Awake()
        {
            base.Awake();

            PageCameraSwiper.OnStartPageChanged += OnChangedThemeAsync;

            stageInfoPanel.gameObject.SetActive(false);

            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        private void OnEnable()
        {
            PageCameraSwiper.IsSwipeable = false;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            PageCameraSwiper.OnStartPageChanged -= OnChangedThemeAsync;
        }


        private void OnChangedThemeAsync(Theme theme)
        {
            stageListPage.DrawButtons(nowTheme = theme, 1);
        }


        private void OnBackButtonClicked()
        {
            if (!stageInfoPanel.IsOpened)
            {
                WindowAnimation.SwipeWindow(stageSelectUIPanel);
                lobbyUIPanel.gameObject.SetActive(true);
                PageCameraSwiper.IsSwipeable = true;
            }
            else
            {
                WindowAnimation.SwipeWindow(stageInfoPanel.transform as RectTransform);
            }
        }


        public static void ShowReadyView(StageData data)
        {
            if (!Instance) return;

            Instance.stageInfoPanel.Show(data);
        }
    }
}
