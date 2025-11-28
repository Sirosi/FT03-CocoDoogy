using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.InGame
{
    // TODO : Undo 아이템 사용 시 이전 행동에 
    public class InGameItemButton : MonoBehaviour
    {
        public CommonButton Button { get; private set; }

        /// <summary>
        /// 해당 버튼이 가지고 있는 ItemData를 InGameItemUI에서 넣어줌
        /// </summary>
        public ItemData ItemData { get; set; }

        public Action<InGameItemButton, ItemData> OnClicked;
        private void Awake()
        {
            if (!Button)
            {
                Button = GetComponent<CommonButton>();
            }

            Button.onClick.AddListener(() => OnClicked?.Invoke(this, ItemData));
        }

        /// <summary>
        /// 아이템 사용 메서드
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public void UseItem(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            if (itemData.effect == ItemEffect.UndoTurn && !PlayerHandler.IsBehaviour)
            {
                MessageDialog.ShowMessage("아이템 사용 실패", "해당 아이템은 1턴 전으로 돌아가는 아이템입니다.\n 진행한 기록이 없으면 사용이 불가합니다.",
                    DialogMode.Confirm, null);
                return;
            }
            
            DataManager.Instance.CurrentItem[itemData] -= 1;
            Button.interactable = false;
            var buttonColor = GetComponent<Image>();
            buttonColor.DOColor(new Color(0.2f, 0.2f, 0.2f), 0.2f);
            
            switch (itemData.effect)
            {
                case ItemEffect.ConsumeAndRecoverMaxAP:
                    Debug.Log("행동력을 1 소모하고 최대 행동력을 1 증가시킵니다.");
                    CommandManager.MaxUp(itemData);
                    break;
                case ItemEffect.RecoverAP:
                    Debug.Log("행동력을 1 증가시킵니다.");
                    CommandManager.Recover(itemData);
                    break;
                case ItemEffect.UndoTurn:
                    Debug.Log("1턴 전으로 돌아갑니다.");
                    CommandManager.Undo(itemData);
                    break;
                case ItemEffect.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// 아이템 구매 메서드 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public async Task PurchaseAsync(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            try
            {
                var result = await FirebaseManager.PurchaseWithCashMoneyAsync(itemData.itemId, 1);

                bool success = result.ContainsKey("success") && (bool)result["success"];

                if (success)
                {
                    Debug.Log($"구매 성공: {itemData.itemName} ({1})");
                    DataManager.Instance.CurrentItem[itemData] += 1;
                }
                else
                {
                    string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                    Debug.LogWarning($"구매 실패: {reason}");
                    MessageDialog.ShowMessage("구매실패", reason, DialogMode.Confirm, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"구매 실패: {e.Message}");
            }
        }
    }
}