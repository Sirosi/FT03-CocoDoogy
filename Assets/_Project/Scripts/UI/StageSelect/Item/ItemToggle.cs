using CocoDoogy.Network;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.StageSelect.Item
{
    public class ItemToggle : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Toggle itemToggle;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        /// <summary>
        /// 현재 해당 토글이 가리키는 아이템 데이터, 사용 안하면 삭제.
        /// </summary>
        public IDictionary<string, object> Data { get; private set; }
        /// <summary>
        /// 토글의 값이 변경될 때 사용되는 이벤트
        /// </summary>
        public event Action<ItemToggle, bool> OnToggleChanged;
        
        /// <summary>
        /// 현재 해당 토글이 가리키는 아이템의 수량
        /// </summary>
        public int CurrentAmount {get; private set;}
        
        public string ItemAmountText {
            set => itemAmountText.text = value;
        }

        private string ItemId;
        
        
        
        private void Awake()
        {
            // 실수로 Inspector에서 연결을 안했을 때를 대비한 코드 
            if (!itemToggle) itemToggle = GetComponent<Toggle>();
            if (!itemAmountText) itemAmountText = GetComponentInChildren<TextMeshProUGUI>();
            
            itemToggle.onValueChanged.AddListener(isOn =>
            {
                OnToggleChanged?.Invoke(this, isOn);
            });
        }

        public void Init(IDictionary<string, object> data, string itemId)
        {
            Data = data;
            ItemId = itemId;
            if (data.TryGetValue(itemId, out object value))
            {
                CurrentAmount = Convert.ToInt32(value);   
            }
            itemAmountText.text = $"{CurrentAmount} 개";
            
            // 아이템 수량이 0이면 토글 비활성화
            itemToggle.interactable = CurrentAmount > 0;
        }

        public async void Use()
        {
            if (!itemToggle.isOn) return;

            IDictionary<string, object> result = await FirebaseManager.Instance.UseItemAsync(ItemId);
            bool success = (bool)result["success"];
            if (success)
            {
                // TODO : 아이템 사용 시 인게임에서 어떻게 나올지 
                Debug.Log("아이템 사용 성공");
            }
            else
            {
                Debug.Log("아이템 사용 실패");
            }
        }
    }
}
