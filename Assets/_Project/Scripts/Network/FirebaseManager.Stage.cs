using Firebase.Firestore;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.Network
{
    public partial class FirebaseManager
    {
        // TODO : 스테이지 관련 Firebase Functions 사용하는 메서드 추가 예정
        
        public async Task<string> GetStageClearedInfo(int theme, int level)
        {
            try
            {
                DocumentReference docRef = Firestore
                    .Collection(Auth.CurrentUser.UserId)
                    .Document("stageInfo")
                    .Collection($"{theme:X2}{level:X2}")
                    .Document("stageClearedInfo");
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            
                if (snapshot.Exists)
                {
                    string uid = snapshot.GetValue<string>("uid");
                    return uid;
                }

                // Debug.LogWarning($"'{nickname}' 닉네임을 찾을 수 없습니다.");
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