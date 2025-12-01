using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class ReceivedRequestPanel : RequestPanel
    {
        /// <summary>
        /// 친구 추가를 수락하는 메서드
        /// </summary>
        /// <param name="uid"></param>
        private async void OnAcceptRequestAsync(string uid)
        {
            var result = await FirebaseManager.CallFriendFunctionAsync("receiveFriendsRequest", uid, "친구 요청 수락 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 추가 수락 성공", "해당 유저를 친구 추가 했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 추가 수락 실패", reason, DialogMode.Confirm, null);
            }
        }

        /// <summary>
        /// 친구 추가를 거절하는 메서드
        /// </summary>
        /// <param name="uid"></param>
        private async void OnRejectRequestAsync(string uid)
        {
            var result = await FirebaseManager.CallFriendFunctionAsync("rejectFriendsRequest", uid, "친구 요청 거절 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 추가 거절 성공", "해당 유저를 친구 추가 거절했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 추가 거절 실패", reason, DialogMode.Confirm, null);
            }
        }

        protected override async Task RefreshPanelAsync()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            var requestDict = await FirebaseManager.GetFriendRequestsAsync("friendReceivedList");
            foreach (var kvp in requestDict)
            {
                string uid = kvp.Key;
                string nickname = kvp.Value;
                var item = Instantiate(prefabItem, container);
                item.GetComponent<FriendRequestItem>().ReceivedInit(nickname, uid, OnAcceptRequestAsync, OnRejectRequestAsync);
            }

            if (requestDict.Count < 1)
            {
                nullMessage.gameObject.SetActive(true);
                nullMessage.text = "받은 친구 요청이 없습니다.";
            }
            else
            {
                nullMessage.gameObject.SetActive(false);
            }
        }
    }
}