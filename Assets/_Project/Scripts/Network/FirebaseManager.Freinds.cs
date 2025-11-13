using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        public async Task<IDictionary<string, object>> CallFriendFunctionAsync(string functionName, string friendsUid, string errorMessage)
        {
            try
            {
                var data = new Dictionary<string, object> { { "friendsUid", friendsUid } };
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
        
        
        /// <summary>
        /// 입력받은 닉네임을 기준으로 파이어베이스 내에 입력받은 닉네임과 같은 닉네임이 있으면 해당 닉네임의 UID 를 반환 
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public async Task<string> FindUserByNicknameAsync(string nickname)
        {
            try
            {
                var docRef = Firestore.Collection("nicknames").Document(nickname);
                var snapshot = await docRef.GetSnapshotAsync();
            
                if (snapshot.Exists)
                {
                    string uid = snapshot.GetValue<string>("uid");
                    return uid;
                }

                Debug.LogWarning($"'{nickname}' 닉네임을 찾을 수 없습니다.");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 검색 실패: {e.Message}");
                return null;
            }
        }
        
        
        /// <summary>
        /// 현재 로그인한 유저가 받은 친구 추가 요청을 모두 Dictionary에 담아 반환 
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetFriendRequestsAsync(string request)
        {
            try
            {
                var userDoc = Firestore
                    .Collection("users").Document(Auth.CurrentUser.UserId)
                    .Collection("private").Document("data");
                
                var snapshot = await userDoc.GetSnapshotAsync();

                if (snapshot.Exists && snapshot.TryGetValue(request, out Dictionary<string, object> dictionary))
                {
                    var result = new Dictionary<string, string>();
                    foreach (var key in dictionary)
                    {
                        if (key.Value is Dictionary<string, object> friendData && friendData.TryGetValue("nickName", out object nickname))
                        {
                            result[key.Key] = nickname.ToString();
                        }
                        else if (key.Value is Dictionary<string, object> giftData && giftData.TryGetValue("giftList", out object gift))
                        {
                            result[key.Key] = gift.ToString();
                        } 
                        else if (key.Value is string value)
                        {
                            result[key.Key] = value;
                        }
                    }
                    return result;
                }
                return new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                Debug.LogError($"친구 요청 목록 불러오기 실패: {e.Message}");
                return new Dictionary<string, string>();
            }
        }
    }
}
