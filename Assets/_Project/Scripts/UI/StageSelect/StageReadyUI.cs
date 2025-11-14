using CocoDoogy.Data;
using CocoDoogy.Network;
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
        
        [Header("Items")]
        [SerializeField] private Toggle[] itemToggles;
        private TextMeshProUGUI[] itemAmounts;
        
        [Header("Item Dictionaries")]
        private IDictionary<string, object> itemDic;
        private long[] itemCounts;
        private bool[] isEquipped;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;

        private void Awake()
        {
            itemAmounts = new TextMeshProUGUI[itemToggles.Length];
            itemCounts = new long[itemToggles.Length];
            isEquipped = new bool[itemToggles.Length];
            
            for (int i = 0; i < itemToggles.Length; ++i)
            {
                int index = i;
                
                itemAmounts[i] = itemToggles[i].GetComponentInChildren<TextMeshProUGUI>();
                isEquipped[i] = false;

                itemToggles[i].onValueChanged.AddListener(isOn => OnItemEquipped(index, isOn));
            }
            
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
            
            
            itemDic = await FirebaseManager.Instance.GetItemListAsync();
            for (int i = 0; i < itemToggles.Length; ++i)
            {
                string key = $"item00{i + 1}";
                long count = (long)itemDic[key];
                
                itemCounts[i] = count;
                isEquipped[i] = false;

                itemToggles[i].SetIsOnWithoutNotify(false);
                itemAmounts[i].text = $"{count}개";
            }


            StageInfoHelps();
        }



        private void OnItemEquipped(int index, bool isOn)
        {
            isEquipped[index] = isOn;
            
            long count = itemCounts[index];
            
            if (isOn)
            {
                itemAmounts[index].text = $"{count - 1}개";
            }
            else
            {
                itemAmounts[index].text = $"{count}개";
            }
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
