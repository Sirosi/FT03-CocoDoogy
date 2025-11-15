using CocoDoogy.Data;
using CocoDoogy.Network;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.StageSelect.Item
{
    public class ItemToggleHandler : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private ItemToggle[] itemToggles;
        
        /// <summary>
        /// 스테이지를 선택하면 초기화를 하도록 OnEnable에서 초기화를 담당<br/>
        /// </summary>
        private void OnEnable()
        {
            InitAsync();
        }
        
        // TODO : 경민씨가 만들어둔 기존 코드는 StageReadyUI에서 모든 기능을 다 하고 있는데 토글버튼이 하는 일을 분할해서 여기에 만들어야 함.
        //        1. 아이템 데이터를 읽어와서 여기서 적용하기 (Clear)
        //        2. 토글이 On이 되어있으면 아이템 사용개수를 -1, Off라면 원상태로 복구 (Clear)
        //        3. 실제로 사용되는 시점은 InGame 스테이지로 들어갔을 때 적용되도록 (Not Clear)
        //        4. 지금 StageReadyUI에서는 isEquipped라는 bool배열로 아이템 사용을 관리하는거 같은데 이거 지우고 여기서 Toggle의 isOn을 확인해서 반환하면 될 듯함. (Not Clear)
        //        지금 생각나는 부분은 이정도로 더 생각나는 부분이 있으면 추가 예정

        /// <summary>
        /// 스테이지 버튼을 클릭해서 StageUI가 나오게 되면 UI를 초기화 하는 메서드
        /// </summary>
        private async void InitAsync()
        {
            var itemDictionary = await FirebaseManager.Instance.GetItemListAsync();
            var itemData = DataManager.Instance.ItemData;
            for (int i = 0; i < itemDictionary.Count; i++)
            {
                string itemId = itemData[i].itemId;
                itemToggles[i].Init(itemDictionary, itemId);
                itemToggles[i].OnToggleChanged += OnClickToggle;
            }
        }
        /// <summary>
        /// 토글 클릭 시 발생하는 이벤트 메서드
        /// </summary>
        /// <param name="toggle">클릭된 토글 버튼</param>
        /// <param name="isOn">토글의 현재 상태</param>
        private void OnClickToggle(ItemToggle toggle, bool isOn)
        {
            toggle.ItemAmountText = isOn ? $"{toggle.CurrentAmount - 1} 개" : $"{toggle.CurrentAmount} 개";
        }
    }
}
