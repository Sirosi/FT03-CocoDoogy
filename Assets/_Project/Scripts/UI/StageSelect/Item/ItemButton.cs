using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.UIManager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.StageSelect.Item
{
    public class ItemButton : MonoBehaviour
    {
        [Header("Item Buttons")]
        [SerializeField] private CommonButton[] itemButton;
        
        [Header("Item Data")]
        [SerializeField] private ItemData[] itemData;
        
        [Header("Item Panel")]
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform itemPanel;
        [SerializeField] private Image itemPanelImage;
        [SerializeField] private TextMeshProUGUI itemPanelText;
        
        [Header("Open Shop")]
        [SerializeField] private CommonButton openItemshopButton;



        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            openItemshopButton.onClick.AddListener(() => LobbyUIManager.Instance.ShopUI.OpenItemShopUI());

            itemPanel.gameObject.SetActive(false);
            
            for (int i = 0; i < itemButton.Length; ++i)
            {
                int index = i;
                itemButton[i].onClick.AddListener(()=> OnItemInfoButtonClicked(index));
            }
        }

        private void OnCloseButtonClicked()
        {
            WindowAnimation.CloseWindow(itemPanel);
        }

        
        private void OnItemInfoButtonClicked(int index)
        {
            itemPanel.gameObject.SetActive(true);
            itemPanelImage.sprite = itemData[index].itemSprite;
            itemPanelText.text = itemData[index].itemDescription;
        }
    }
}
