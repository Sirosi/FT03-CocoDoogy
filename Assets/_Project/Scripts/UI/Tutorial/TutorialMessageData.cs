using UnityEngine;

namespace CocoDoogy.UI.Tutorial
{
    /// <summary>
    /// 튜토리얼 메시지 데이터 (ScriptableObject)
    /// 배열로 여러 메시지를 저장하여 순차적으로 표시
    /// </summary>
    [CreateAssetMenu(fileName = "TutorialMessageData", menuName = "Scriptable Objects/Tutorial/MessageData")]
    public class TutorialMessageData : ScriptableObject
    {
        [Header("Tutorial Settings")]
        [Tooltip("튜토리얼 고유 ID (처음만 표시하는 로직에 사용)")]
        public string tutorialID;

        [Tooltip("표시할 메시지 배열 (순서대로 표시됨)")]
        [TextArea(3, 10)]
        public string[] messages;
    }
}

