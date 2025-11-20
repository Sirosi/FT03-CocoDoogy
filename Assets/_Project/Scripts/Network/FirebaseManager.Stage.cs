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
                    .GetSnapshotAsync();

                // TODO : 이거 무조건 1개만 가져와서 그걸 기준으로 하기 떄문에 어떤걸 넣던 1-1만 나옴 수정해야함.
                var lastStageDoc = snapshot.Documents
                    .Select(doc => new {
                        Doc = doc,
                        IdValue = Convert.ToInt32(doc.Id, 16),
                    })
                    .OrderByDescending(x => x.IdValue)
                    .FirstOrDefault()?.Doc;
                
                if (lastStageDoc is { Exists: true })
                {
                    DocumentSnapshot doc = snapshot.Documents.FirstOrDefault();
                    // Firestore 데이터를 StageInfo 객체로 변환
                    if (doc != null)
                    {
                        StageInfo stage = doc.ConvertTo<StageInfo>();
                        StageSelectManager.LastClearedStage = stage;
                    }
                }
                else
                {
                    StageSelectManager.LastClearedStage = null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"사용자 검색 실패: {e.Message}");
            }
        }
    }
}