using CocoDoogy.Core;
using UnityEngine;

namespace CocoDoogy.Data
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Data/Stage Data")]
    public class StageData : ScriptableObject
    {
        public Theme theme = Theme.None;
        public int index = 0;
        
        public string stageName;
        public TextAsset mapData;

        public int[] starThresholds;
    }
}
