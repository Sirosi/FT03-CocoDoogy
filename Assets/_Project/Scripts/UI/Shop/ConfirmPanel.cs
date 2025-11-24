using CocoDoogy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.CameraSwiper.Shop
{
    public class ConfirmPanel : MonoBehaviour
    {
        [SerializeField] private Image purchasedItemImage;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private CommonButton confirmButton;

        private void Awake()
        {
            confirmButton.onClick.AddListener(() =>
            {
                WindowAnimation.CloseWindow(transform);
            });
        }

        public void Open(ItemData itemData, int quantity)
        {
            purchasedItemImage.sprite = itemData.itemSprite;
            quantityText.text = $"x{quantity}";
            gameObject.SetActive(true);
        }
    }
}