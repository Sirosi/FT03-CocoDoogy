using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class FriendsInfoPanel : RequestPanel
    {
        private async void OnDeleteRequestAsync(string uid)
        {
            IDictionary<string, object> result = await FirebaseManager.CallFriendFunctionAsync("deleteFriendsRequest", uid, "친구 삭제 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 삭제 성공","친구 삭제에 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.TryGetValue("reason", out object value) ? value.ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 삭제 실패", reason, DialogMode.Confirm, null);
            }
        }
        
        private async void OnGiftRequestAsync(string uid)
        {
            IDictionary<string, object> result = await FirebaseManager.CallFriendFunctionAsync("giftFriendsRequest", uid, "선물 보내기 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("선물 보내기 성공","선물 보내기를 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.TryGetValue("reason", out object value) ? value.ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("선물 보내기 실패", reason, DialogMode.Confirm, null);
            }
        }
        
        protected override async Task RefreshPanelAsync()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            var requestDict = await FirebaseManager.GetFriendRequestsAsync("friendsList");
            foreach ((string uid, string nickname) in requestDict)
            {
                FriendRequestItem item = Instantiate(prefabItem, container);
                item.GetComponent<FriendRequestItem>().FriendInit(nickname, uid, OnGiftRequestAsync,OnDeleteRequestAsync);
            }
            
            if (requestDict.Count < 1)
            {
                nullMessage.gameObject.SetActive(true);
                nullMessage.text = "이런, 나는 친구가 없습니다!";
            }
            else
            {
                nullMessage.gameObject.SetActive(false);
            }
        }
    }
}