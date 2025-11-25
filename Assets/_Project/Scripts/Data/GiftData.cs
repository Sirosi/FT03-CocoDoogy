using Firebase.Firestore;

namespace CocoDoogy._Project.Scripts.Data
{
    [FirestoreData]
    public class GiftData
    {
        [FirestoreProperty] public string fromNickname { get; set; }
        [FirestoreProperty] public string giftType { get; set; }
        [FirestoreProperty] public string giftId { get; set; }
        [FirestoreProperty] public int giftCount { get; set; }
        [FirestoreProperty] public bool isClaimed { get; set; }
        [FirestoreProperty] public long sentAt { get; set; }
    }
}