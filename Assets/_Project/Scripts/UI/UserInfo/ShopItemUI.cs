using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.Network.Purchase;
using CocoDoogy.CameraSwiper;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO : 지금은 사용하지 않음 => ShopItem으로 대체
namespace CocoDoogy.CameraSwiper.UserInfo
{
    public class ShopItemUI : MonoBehaviour
    {
        [Header("Item Data")]
        [SerializeField] private ItemData itemData;

        [Header("Item Information")]
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemPrice;
        [SerializeField] private CommonButton openDescriptionButton;

        [Header("Description Window")]
        [SerializeField] private RectTransform descriptionWindow;
        [SerializeField] private Image descriptionImage;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Confirm Window")]
        [SerializeField] private RectTransform confirmWindow;
        [SerializeField] private Image purchasedImage;
        [SerializeField] private TextMeshProUGUI purchasedQuantity;

        [Header("Control the quantities")]
        [SerializeField] private TextMeshProUGUI currentQuantity;
        [SerializeField] private CommonButton minusTenButton;
        [SerializeField] private CommonButton minusOneButton;
        [SerializeField] private CommonButton plusOneButton;
        [SerializeField] private CommonButton plusTenButton;
        private int counter;

        [Header("Answer")]
        [SerializeField] private CommonButton buyButton;
        [SerializeField] private CommonButton cancelButton;
        [SerializeField] private CommonButton confirmButton;

        private PurchaseButtonHandler handler;

        private void Awake()
        {
            openDescriptionButton.onClick.AddListener(OnMoreButtonClicked);


            minusTenButton.onClick.AddListener(OnMinusTenButtonClicked);
            minusOneButton.onClick.AddListener(OnMinusOneButtonClicked);
            plusOneButton.onClick.AddListener(OnPlusOneButtonClicked);
            plusTenButton.onClick.AddListener(OnPlusTenButtonClicked);


            buyButton.onClick.AddListener(OnBuyButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        private void OnEnable()
        {
            itemImage.sprite = itemData.itemSprite;
            itemName.text = itemData.itemName;
            itemPrice.text = itemData.purchasePrice.ToString();
        }

        private void OnMoreButtonClicked()
        {
            counter = 1;
            currentQuantity.text = counter.ToString();

            handler = descriptionWindow.GetComponent<PurchaseButtonHandler>();
            handler.ItemId = itemData.itemId;
            handler.ShopItemUI = this;

            descriptionImage.sprite = itemData.itemSprite;
            descriptionText.text = itemData.itemDescription;
            descriptionWindow.gameObject.SetActive(true);
        }
        private void OnBuyButtonClicked()
        {
            purchasedImage.sprite = descriptionImage.sprite;
            purchasedQuantity.text = $"X{counter.ToString()}";

            confirmWindow.gameObject.SetActive(true);
        }
        private void OnCancelButtonClicked()
        {
            WindowAnimation.CloseWindow(descriptionWindow);
        }
        private void OnConfirmButtonClicked()
        {
            confirmWindow.gameObject.SetActive(false);
            WindowAnimation.CloseWindow(descriptionWindow);
        }

        #region Quantity Controller
        private void QuantityChanged(int adds)
        {
            counter += adds;
            if (counter <= 1) counter = 1;
            if (counter >= 100) counter = 100;
            currentQuantity.text = counter.ToString();
        }

        private void OnMinusTenButtonClicked()
        {
            QuantityChanged(-10);
        }
        private void OnMinusOneButtonClicked()
        {
            QuantityChanged(-1);
        }
        private void OnPlusOneButtonClicked()
        {
            QuantityChanged(+1);
        }
        private void OnPlusTenButtonClicked()
        {
            QuantityChanged(+10);
        }

        #endregion
    }
}
