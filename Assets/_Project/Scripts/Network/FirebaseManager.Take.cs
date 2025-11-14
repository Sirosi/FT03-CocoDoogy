using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        /// <summary>
        /// 아이템 ID를 기준으로 아이템을 구매하는 메서드 <br/>
        /// 아이템 ID가 이상하면 에러 메세지 출력 <br/>
        /// 아이템 ID가 정상이고 돈이 부족하면 return dict { success: false, reason: "돈부족" } <br/>
        /// 아이템 ID가 정상이고 돈이 부족하지 않으면 return dict{ success: true, ... }
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IDictionary<string, object>> PurchaseWithCashMoneyAsync(string itemId, int quantity)
        {
            if (!IsFirebaseReady) throw new Exception("Firebase가 초기화되지 않았습니다.");
            var data = new Dictionary<string, object>
            {
                { "itemId", itemId }, { "itemQuantity", Convert.ToInt32(quantity) }
            };
            try
            {
                var result = await Functions.GetHttpsCallable("purchaseWithCashMoney").CallAsync(data);
                string json = JsonConvert.SerializeObject(result.Data);
                var dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
                return dict;
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase Function 호출 실패: {e.Message}");
                throw;
            }
        }

        public async Task<IDictionary<string, object>> TakePresentRequestAsync(string functionName, string giftId,
            string errorMessage)
        {
            try
            {
                var data = new Dictionary<string, object> { { "giftId", giftId } };
                var result = await Functions.GetHttpsCallable(functionName).CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"{errorMessage}: {e.Message}");
                throw;
            }
        }

        public async Task<List<Dictionary<string, object>>> GetGiftListAsync()
        {
            var userId = Auth.CurrentUser.UserId;
            var userDoc = await Firestore
                .Collection("users")
                .Document(userId)
                .Collection("private")
                .Document("data")
                .GetSnapshotAsync();

            if (!userDoc.Exists) return new List<Dictionary<string, object>>();

            if (userDoc.TryGetValue("giftList", out List<object> giftListRaw))
            {
                var giftList = new List<Dictionary<string, object>>();
                foreach (var item in giftListRaw)
                {
                    giftList.Add(item as Dictionary<string, object>);
                }

                return giftList;
            }

            return new List<Dictionary<string, object>>();
        }
        
        /// <summary>
        /// Firebase Firestore에서 현재 로그인한 유저의 itemDic을 읽어와 반환하는 메서드
        /// </summary>
        public async Task<IDictionary<string, object>> GetItemListAsync()
        {
            var userId = Auth.CurrentUser.UserId;
            var userDoc = await Firestore
                .Collection("users")
                .Document(userId)
                .Collection("private")
                .Document("data")
                .GetSnapshotAsync();
            if (!userDoc.Exists) return new Dictionary<string, object>();
            if (userDoc.TryGetValue("itemDic", out Dictionary<string, object> dictionary))
            {
                var itemDic = new Dictionary<string, object>();
                foreach (var item in dictionary)
                {
                    itemDic[item.Key] = item.Value;
                }
                return itemDic;
            }

            return new Dictionary<string, object>();
        }
    }
}