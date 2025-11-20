using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        // TODO : 스테이지 관련 Firebase Functions 사용하는 메서드 추가 예정

        /// <summary>
        /// 스테이지를 클리어하면 작동하는 메서드.
        /// </summary>
        /// <returns></returns>
        public static async Task<IDictionary<string, object>> ClearStageAsync(int theme, int level, int remainAP,
            float clearTime)
        {
            try
            {
                Dictionary<string, object> data = new()
                {
                    { "theme", Extensions.Hex2(theme) },
                    { "level", Extensions.Hex2(level) },
                    { "remainAP", remainAP },
                    { "clearTime", clearTime }
                };
                HttpsCallableResult result = await Instance.Functions.GetHttpsCallable("clearStage").CallAsync(data);

                string json = JsonConvert.SerializeObject(result.Data);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"스테이지 클리어 저장 실패: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 클리어한 스테이지 정보를 찾아 가장 마지막으로 클리어한 스테이지를 반환.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetLastClearStage()
        {
            try
            {
                var snapshot = await Instance.Firestore
                    .Collection($"users/{Instance.Auth.CurrentUser.UserId}/stageInfo")
                    .WhereEqualTo("cleared", true)
                    .OrderByDescending(FieldPath.DocumentId)
                    .Limit(1)
                    .GetSnapshotAsync();
                foreach (var snap in snapshot)
                {
                    Debug.Log(snap.Id);
                }

                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 검색 실패: {e.Message}");
                return null;
            }
        }
    }
}