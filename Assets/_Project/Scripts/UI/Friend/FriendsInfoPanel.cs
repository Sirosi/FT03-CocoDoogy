using CocoDoogy.UI.Popup;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI.Friend
{
    public class FriendsInfoPanel : RequestPanel
    {
        private async void OnDeleteRequestAsync(string uid)
        {
            var result = await Firebase.CallFriendFunctionAsync("deleteFriendsRequest", uid, "친구 삭제 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("친구 삭제 성공","친구 삭제에 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("친구 삭제 실패", reason, DialogMode.Confirm, null);
            }
        }
        
        private async void OnPresentRequestAsync(string uid)
        {
            var result = await Firebase.CallFriendFunctionAsync("presentFriendsRequest", uid, "선물 보내기 실패");
            bool success = (bool)result["success"];

            if (success)
            {
                MessageDialog.ShowMessage("선물 보내기 성공","선물 보내기를 성공했습니다.", DialogMode.Confirm, null);
            }
            else
            {
                string reason = result.ContainsKey("reason") ? result["reason"].ToString() : "알 수 없는 이유";
                MessageDialog.ShowMessage("선물 보내기 실패", reason, DialogMode.Confirm, null);
            }
        }
        
        protected override async Task RefreshPanelAsync()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            var requestDict = await Firebase.GetFriendRequestsAsync("friendsList");
            foreach (var kvp in requestDict)
            {
                string uid = kvp.Key;
                string nickname = kvp.Value;
                var item = Instantiate(prefabItem, container);
                item.GetComponent<FriendRequestItem>().FriendInit(nickname, uid, OnPresentRequestAsync,OnDeleteRequestAsync);
            }
        }
    }
}