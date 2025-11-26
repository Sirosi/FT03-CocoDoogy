using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.UI.Shop.Category
{
    public class ShopCategory : MonoBehaviour
    {
        [Header("Purchase Item Button")]
        [SerializeField] private List<ShopItem> shopItems;
        private ShopUI shopUI;

        private void Awake()
        {
            shopUI = GetComponentInParent<ShopUI>();

            foreach (var shopItem in shopItems)
            {
                shopItem.OnClickSubscriptionEvent((data, isSaleforCash) => shopUI.OpenPurchasePanel(data, isSaleforCash));
            }
        }

        public void Change(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}