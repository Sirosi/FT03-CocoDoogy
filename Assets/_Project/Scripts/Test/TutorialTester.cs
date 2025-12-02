using CocoDoogy.UI.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.Test
{
    /// <summary>
    /// 튜토리얼 UI 테스트 스크립트
    /// 키보드 입력으로 튜토리얼을 테스트할 수 있습니다
    /// </summary>
    public class TutorialTester : MonoBehaviour
    {
        [SerializeField] private TutorialMessageData[] testTutorials;
        [SerializeField] private TutorialUI tutorialUI;

        private void Awake()
        {
            // TutorialUI 자동 찾기
            if (tutorialUI == null)
            {
                tutorialUI = FindFirstObjectByType<TutorialUI>();
            }

            if (tutorialUI == null)
            {
                Debug.LogWarning("[TutorialTester] TutorialUI를 찾을 수 없습니다. Inspector에서 수동으로 할당해주세요.");
            }
        }

        void Start()
        {
            ResetPlayerPrefs();
            tutorialUI.ShowTutorial(testTutorials[0]);
        }

        /// <summary>
        /// PlayerPrefs 초기화 (튜토리얼 표시 플래그 리셋)
        /// </summary>
        private void ResetPlayerPrefs()
        {
            if (testTutorials == null || testTutorials.Length == 0)
            {
                Debug.LogWarning("[TutorialTester] 초기화할 튜토리얼 데이터가 없습니다.");
                return;
            }

            int resetCount = 0;
            foreach (var tutorial in testTutorials)
            {
                if (tutorial != null && !string.IsNullOrEmpty(tutorial.tutorialID))
                {
                    string key = $"TutorialShown_{tutorial.tutorialID}";
                    PlayerPrefs.DeleteKey(key);
                    resetCount++;
                }
            }

            PlayerPrefs.Save();
            Debug.Log($"[TutorialTester] PlayerPrefs 초기화 완료 ({resetCount}개 튜토리얼 플래그 삭제)");
        }

        #region Inspector 버튼용 메서드

        #endregion
    }
}

