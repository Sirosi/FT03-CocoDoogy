using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI;
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
        private Image[] itemImage;
        private TextMeshProUGUI[] itemAmountText;
        
        [Header("Item Panel")]
        [SerializeField] private GameObject itemPanel;
        [SerializeField] private Image itemPanelImage;
        [SerializeField] private TextMeshProUGUI itemPanelText;



        private void Awake()
        {
            for (int i = 0; i < itemButton.Length; ++i)
            {
                itemImage[i] = itemButton[i].GetComponent<Image>();
                itemAmountText[i] = itemButton[i].GetComponentInChildren<TextMeshProUGUI>();
            }
            
            
        }

        
    }
}
