using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.UI.Tutorial
{
    /// <summary>
    /// 튜토리얼 UI 컨트롤러
    /// 클릭 시 다음 메시지로 넘어가고, 처음만 표시하는 로직 포함
    /// PlayerPrefs로 로컬에서만 관리
    /// </summary>
    public class TutorialUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image backgroundImage;

        [Header("Settings")]
        [Tooltip("기본 알파값 (0~1)")]
        [Range(0f, 1f)]
        [SerializeField] private float defaultAlpha = 1f;

        [Tooltip("페이드 아웃 시간")]
        [SerializeField] private float fadeDuration = 0.3f;

        private TutorialMessageData currentTutorialData;
        private int currentIndex = 0;
        private bool isSequenceActive = false;

        private void OnEnable()
        {
            ResetAlpha();
        }

        /// <summary>
        /// 튜토리얼 표시 시작
        /// </summary>
        /// <param name="tutorialData">표시할 튜토리얼 데이터</param>
        public void ShowTutorial(TutorialMessageData tutorialData)
        {
            if (tutorialData == null)
            {
                Debug.LogWarning("[TutorialUI] 튜토리얼 데이터가 없습니다.");
                return;
            }

            if (tutorialData.messages == null || tutorialData.messages.Length == 0)
            {
                Debug.LogWarning("[TutorialUI] 메시지가 없습니다.");
                return;
            }

            // 처음만 표시하는 로직 체크 (PlayerPrefs로 로컬 관리)
            if (!string.IsNullOrEmpty(tutorialData.tutorialID))
            {
                string key = $"TutorialShown_{tutorialData.tutorialID}";
                if (PlayerPrefs.GetInt(key, 0) == 1)
                {
                    // 이미 표시된 튜토리얼이면 표시하지 않음
                    return;
                }
            }

            currentTutorialData = tutorialData;
            currentIndex = 0;
            isSequenceActive = true;

            ShowCurrentMessage();
        }

        /// <summary>
        /// 현재 인덱스의 메시지 표시
        /// </summary>
        private void ShowCurrentMessage()
        {
            if (!isSequenceActive ||
                currentTutorialData == null ||
                currentIndex >= currentTutorialData.messages.Length)
            {
                EndSequence();
                return;
            }

            if (descriptionText == null || backgroundImage == null)
            {
                Debug.LogError("[TutorialUI] descriptionText 또는 backgroundImage가 설정되지 않았습니다.");
                return;
            }

            // 게임오브젝트 활성화
            gameObject.SetActive(true);

            // 텍스트 설정
            descriptionText.text = currentTutorialData.messages[currentIndex];

            // 알파값 초기화
            ResetAlpha();

            // 진행 중인 애니메이션 중단
            DOTween.Kill(descriptionText);
            DOTween.Kill(backgroundImage);
        }

        /// <summary>
        /// 알파값 초기화
        /// </summary>
        private void ResetAlpha()
        {
            if (backgroundImage != null)
            {
                var bgColor = backgroundImage.color;
                bgColor.a = defaultAlpha;
                backgroundImage.color = bgColor;
            }
        }

        /// <summary>
        /// 클릭 이벤트 처리 - 다음 메시지로 이동
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isSequenceActive || !gameObject.activeSelf)
                return;

            // 다음 메시지로 이동
            currentIndex++;

            if (currentIndex >= currentTutorialData.messages.Length)
            {
                // 모든 메시지 표시 완료
                EndSequence();
            }
            else
            {
                // 다음 메시지 표시
                ShowCurrentMessage();
            }
        }

        /// <summary>
        /// 시퀀스 종료
        /// </summary>
        private void EndSequence()
        {
            isSequenceActive = false;

            // 처음만 표시하는 로직 - 표시 완료 플래그 저장
            if (currentTutorialData != null && !string.IsNullOrEmpty(currentTutorialData.tutorialID))
            {
                string key = $"TutorialShown_{currentTutorialData.tutorialID}";
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
            }

            // 페이드 아웃 후 비활성화
            if (descriptionText != null && backgroundImage != null)
            {
                descriptionText.DOFade(0f, fadeDuration);
                backgroundImage.DOFade(0f, fadeDuration)
                    .OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 외부에서 시퀀스 중단
        /// </summary>
        public void StopTutorial()
        {
            EndSequence();
        }
    }
}

