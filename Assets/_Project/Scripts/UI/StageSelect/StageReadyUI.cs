using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.StageSelect.Item;
using CocoDoogy.UI;
using CocoDoogy.UI.StageSelect;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect
{
    public class StageReadyUI : MonoBehaviour
    {
        private int selectedStage;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI title;
        
        [Header("Stage Informations")]
        [SerializeField] private RectTransform page1;
        [SerializeField] private RectTransform page2;
        private bool isFirstPage;
        
        [Header("StageInfo Helps")]
        [SerializeField] private RectTransform content;
        [SerializeField] private StageInfo[] stageInfos;
        
        [Header("Ranks")]
        [SerializeField] private GameObject[] ranks;
        private TextMeshProUGUI[] rankTexts;
        private CommonButton[] replayButtons;

        [Header("Item Toggle Handler")]
        [SerializeField] private ItemToggleHandler itemToggleHandler;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;

        private void Awake()
        {
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private async void OnEnable()
        {
            selectedStage = StageSelectManager.SelectedStage;
            title.text = $"Stage{selectedStage}";
            
            isFirstPage = true;
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);
            
            StageInfoHelps();
        }
        
        private async void OnPageChangeButtonClicked()
        {
            if (isFirstPage)
            {
                await WindowAnimation.TurnPage(page1);
                page2.gameObject.SetActive(true);
                isFirstPage = false;
            }
            else if (!isFirstPage)
            {
                await WindowAnimation.TurnPage(page2);
                page1.gameObject.SetActive(true);
                isFirstPage = true;
            }
        }
        
        private void StageInfoHelps()
        {
            StageInfo stageInfo = stageInfos[selectedStage - 1];
            for (int i = content.childCount - 1; i >= 0; --i)
            {
                Destroy(content.GetChild(i).gameObject);
            }
            foreach (var prefab in stageInfo.contentPrefabs)
            {
                Instantiate(prefab, content);
            }
        }
        
        private void OnStartButtonClicked()
        {
            Loading.LoadScene($"InGame");
        }
    }
}
