using CocoDoogy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CocoDoogy.CameraSwiper.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [Header("Purchase Item Info")]
        [SerializeField] private ItemData itemData;

        [Header("Shop Item")]
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemPrice;
        [SerializeField] private CommonButton purchaseButton;

        public ItemData ItemData => itemData;

        private void OnEnable()
        {
            itemImage.sprite = itemData.itemSprite;
            itemName.text = itemData.itemName;
            itemPrice.text = itemData.purchasePrice.ToString();
        }

        /// <summary>
        /// 버튼 이벤트 구독하는 메서드
        /// </summary>
        /// <param name="action"></param>
        public void OnClickSubscriptionEvent(UnityAction action) => purchaseButton.onClick.AddListener(action);
    }
}