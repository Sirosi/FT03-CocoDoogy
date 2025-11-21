using UnityEngine;

namespace CocoDoogy.MiniGame
{
    [CreateAssetMenu(fileName = "TutorialExplainData", menuName = "Scriptable Objects/MiniGame/ExplainData")]
    public class TutorialExplainData : ScriptableObject
    {
        public string title;
        [TextArea(3,3)]
        public string description;
    }
}