using CocoDoogy.UI;
using CocoDoogy.UI.StageSelect;
using System;
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
        
        [Header("Tips")]
        
        
        [Header("Ranks")]
        [SerializeField] private GameObject[] ranks;
        private TextMeshProUGUI[] rankTexts;
        private CommonButton[] replayButtons;
        
        [Header("Items")]
        [SerializeField] private Toggle[] itemToggles;
        private TextMeshProUGUI[] itemAmounts;
        private bool[] isEquipped;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;

        private void Awake()
        {
            itemAmounts = new TextMeshProUGUI[itemToggles.Length];
            isEquipped = new bool[itemToggles.Length];
            
            for (int i = 0; i < itemToggles.Length; ++i)
            {
                int index = i;
                
                itemAmounts[i] = itemToggles[i].GetComponentInChildren<TextMeshProUGUI>();
                isEquipped[i] = itemToggles[i].isOn;

                itemToggles[i].onValueChanged.AddListener(isOn => OnItemEquipped(index, isOn));
            }
            
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnEnable()
        {
            selectedStage = StageSelectManager.SelectedStage;
            title.text = $"Stage{selectedStage}";
            
            isFirstPage = true;
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);
        }



        private void OnItemEquipped(int index, bool isOn)
        {
            if (isOn)
            {
                itemAmounts[index].text = $"{2-1}개";
            }
            else
            {
                itemAmounts[index].text = $"2개";
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
            
        private void OnStartButtonClicked()
        {
            Loading.LoadScene($"Stage{selectedStage}");
        }
    }
}
