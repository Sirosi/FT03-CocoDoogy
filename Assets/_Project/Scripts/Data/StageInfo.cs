using Firebase.Firestore;

namespace CocoDoogy.Data
{
    [FirestoreData]
    public class StageInfo
    {
        [FirestoreProperty] public int remainAp { get; set; }
        [FirestoreProperty] public float cleartime { get; set; }
        [FirestoreProperty] public bool cleared { get; set; }
        [FirestoreProperty] public string theme { get; set; }
        [FirestoreProperty] public string level { get; set; }
        [FirestoreProperty] public string replayData { get; set; }
    }
}