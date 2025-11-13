using UnityEngine;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName = "NewStageInfo", menuName = "Data/StageInfo Data")]
    public class StageInfo : ScriptableObject
    {
        public int stageNumber;
        public GameObject[] contentPrefabs;
    }
}
