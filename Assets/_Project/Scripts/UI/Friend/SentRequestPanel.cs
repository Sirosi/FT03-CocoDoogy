using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class SentRequestPanel : RequestPanel
    {
        /// <summary>
        /// 친구 추가를 취소하는 메서드
        /// </summary>
        /// <param name="uid"></param>
        private async void OnCancelRequestAsync(string uid)
        {
            var result = await FirebaseManager.CallFriendFunctionAsync("cancelFriendsRequest", uid, "친구 요청 취소 실패");

            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 요청 취소 성공","해당 유저에게 보낸 친구 추가 요청을 취소했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 요청 취소 실패", reason, DialogMode.Confirm, null);
            }
        }
        
        protected override async Task RefreshPanelAsync()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            var requestDict = await FirebaseManager.GetFriendRequestsAsync("friendSentList");
            foreach (var kvp in requestDict)
            {
                string uid = kvp.Key;
                string nickname = kvp.Value;
                var item = Instantiate(prefabItem, container);
                item.GetComponent<FriendRequestItem>().SentInit(nickname, uid, OnCancelRequestAsync);
            }
            
            if (requestDict.Count < 1)
            {
                nullMessage.gameObject.SetActive(true);
                nullMessage.text = "새 친구를 찾아볼까요?";
            }
            else
            {
                nullMessage.gameObject.SetActive(false);
            }
        }
    }
}