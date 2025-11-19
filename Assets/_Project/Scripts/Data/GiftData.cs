using Firebase.Firestore;

namespace CocoDoogy._Project.Scripts.Data
{
    [FirestoreData]
    public class GiftData
    {
        [FirestoreProperty] public string FromNickname { get; set; }
        [FirestoreProperty] public string GiftType { get; set; }
        [FirestoreProperty] public string GiftId { get; set; }
        [FirestoreProperty] public int GiftCount { get; set; }
        [FirestoreProperty] public bool IsClaimed { get; set; }
        [FirestoreProperty] public long SentAt { get; set; }
    }
}