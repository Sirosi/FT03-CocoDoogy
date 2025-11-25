using CocoDoogy._Project.Scripts.Data;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Command.Content;
using CocoDoogy.Network;
using CocoDoogy.UI;
using CocoDoogy.UI.Popup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace CocoDoogy
{
    public class InGameItemButton : MonoBehaviour
    {
        private CommonButton button;
        
        public CommonButton Button => button;
        public ItemData ItemData { get; set; }

        public Action<InGameItemButton, ItemData> OnClickEvent;

        private void Awake()
        {
            if (!button)
            {
                button = GetComponent<CommonButton>();
            }

            button.onClick.AddListener(() => OnClickEvent?.Invoke(this, ItemData));
        }

        /// <summary>
        /// 아이템 사용 메서드
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemData"></param>
        public void UseItem(CallbackType type, ItemData itemData)
        {
            if (type != CallbackType.Yes) return;
            if (itemData.effect == ItemEffect.UndoTurn)
            {
                Debug.Log($"CommandManager.Executed.Count: {CommandManager.Executed.Count}");
                if (CommandManager.Executed.Count <= 2)
                {
                    MessageDialog.ShowMessage("아이템 사용 실패", "해당 아이템은 1턴 전으로 돌아가는 아이템입니다.\n 진행한 기록이 없으면 사용이 불가합니다.",
                        DialogMode.Confirm, null);
                    return;
                }
            }

            InGameItemUI.UsedItems[itemData] = true;
            
            // TODO : 지금은 아이템 사용 시 파이어베이스에서 즉각적으로 반응하여 아이템 수량을 감소시키지만
            // Undo 기능을 생각하면 아이템 소모를 게임 종료 시 소모된것만 적용되게 변경해야할듯 함.
            // 아이템을 사용하되 소모되는 시점이 게임 끝나고 로비로 돌아가는 시점으로 하면 될듯.
            // 아이템 1회 사용 후 더이상 사용하지 못하게 변경
            button.interactable = false;
            // TODO : 아이템 사용 시 인게임 기능 추가 
            switch (itemData.effect)
            {
                case ItemEffect.ConsumeAndRecoverMaxAP:
                    Debug.Log("행동력을 1 소모하고 최대 행동력을 1 증가시킵니다.");
                    CommandManager.MaxUp(1);
                    break;
                case ItemEffect.RecoverAP:
                    Debug.Log("행동력을 1 증가시킵니다.");
                    CommandManager.Recover(1);
                    break;
                case ItemEffect.UndoTurn:
                    Debug.Log("1턴 전으로 돌아갑니다.");
                    CommandManager.Undo();
                    break;
                case ItemEffect.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // IDictionary<string, object> result = await FirebaseManager.UseItemAsync(itemData.itemId);
            // bool success = (bool)result["success"];
            // if (success)
            // {
            //     // 아이템 1회 사용 후 더이상 사용하지 못하게 변경
            //     button.interactable = false;
            //     // TODO : 아이템 사용 시 인게임 기능 추가 
            //     switch (itemData.effect)
            //     {
            //         case ItemEffect.ConsumeAndRecoverMaxAP:
            //             Debug.Log("행동력을 1 소모하고 최대 행동력을 1 증가시킵니다.");
            //             CommandManager.MaxUp(1);
            //             break;
            //         case ItemEffect.RecoverAP:
            //             Debug.Log("행동력을 1 증가시킵니다.");
            //             CommandManager.Recover(1);
            //             break;
            //         case ItemEffect.UndoTurn:
            //             Debug.Log("1턴 전으로 돌아갑니다.");
            //             CommandManager.Undo();
            //             break;
            //         case ItemEffect.None:
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // }
            // else
            // {
            //     Debug.Log("아이템 사용 실패");
            // }
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
