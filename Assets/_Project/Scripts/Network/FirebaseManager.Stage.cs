using CocoDoogy.Data;
using CocoDoogy.UI.StageSelect;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    { "theme", theme.Hex2() },
                    { "level", level.Hex2() },
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
        /// 클리어 한 스테이지 정보를 찾아 가장 최근에 클리어한 스테이지를 반환.
        /// </summary>
        /// <returns></returns>
        public static async Task GetLastClearStage(string theme)
        {
            try
            {
                var snapshot = await Instance.Firestore
                    .Collection($"users/{Instance.Auth.CurrentUser.UserId}/stageInfo")
                    .WhereEqualTo("theme", theme)
                    .OrderByDescending(FieldPath.DocumentId)
                    .Limit(1)
                    .GetSnapshotAsync();
                if (snapshot.Documents.Any())
                {
                    var doc = snapshot.Documents.FirstOrDefault();
                    // Firestore 데이터를 StageInfo 객체로 변환
                    StageInfo stage = doc.ConvertTo<StageInfo>();
                    StageSelectManager.LastClearedStage = stage;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 검색 실패: {e.Message}");
            }
        }
    }
}