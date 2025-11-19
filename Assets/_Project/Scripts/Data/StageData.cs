using UnityEngine;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Data/Stage Data")]
    public class StageData : ScriptableObject
    {
        public string stageName;
        public TextAsset mapData;
    }
}
