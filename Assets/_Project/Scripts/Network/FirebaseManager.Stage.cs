using Firebase.Extensions;
using Firebase.Firestore;
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
        /// 클리어한 스테이지 정보를 찾아 가장 마지막으로 클리어한 스테이지를 반환.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLastClearStage()
        {
            try
            {
                var snapshot = await Firestore
                    .Collection($"users/{Auth.CurrentUser.UserId}/stageInfo")
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
        
        /// <summary>
        /// 스테이지 정보를 FirebaseStore에 생성하는 테스트 코드
        /// </summary>
        /// <param name="uid"></param>
        public void SeedStages(string uid)
        {
            for (int i = 1; i <= 10; i++)
            {
                string stageId = $"01{i:D2}"; // 0101, 0102, ...
                DocumentReference docRef = Firestore
                    .Collection($"users/{uid}/stageInfo")
                    .Document(stageId);

                Dictionary<string, object> stageData = new Dictionary<string, object>
                {
                    { "clearTime", 0f },
                    { "remainAP", 10 },
                    { "replayData", "" },
                    { "stageId", i },
                    { "theme", 1 },
                    { "cleared", false }
                };

                docRef.SetAsync(stageData).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                        Debug.Log($"Stage {stageId} created successfully.");
                    else
                        Debug.LogError($"Failed to create stage {stageId}: {task.Exception}");
                });
            }
        }
    }
}