using UnityEngine;
using DG.Tweening;

namespace CocoDoogy.EmoteBillboard
{
    // 어느 방향에서든 이모지가 제대로 보이도록 하는 스크립트
    public class EmoteBillboard : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("이모트를 띄울 타겟 오브젝트")]
        [SerializeField] private Transform target; // 타겟 오브젝트
        [Tooltip("타겟과의 거리 오프셋")]
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

        [Header("Animation Settings")]
        [Tooltip("위로 떠올랐다가 사라지는 시간")]
        [SerializeField] private float duration = 5f;
        [Tooltip("사라지는 페이드 시간")]
        [SerializeField] private float fadeTime = 0.15f;

        private Camera mainCamera; // 메인 카메라 참조
        private SpriteRenderer spriteRenderer; // 이모트의 스프라이트 렌더러
        private Vector3 startPos; // 이모트의 초기 위치
        private Vector3 originalScale; // 이모트의 원래 크기
        private Sequence currentSequence; // 이모트가 움직이는 애니메이션을 위한 시퀀스

        private void Start()
        {
            mainCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;

            ShowEmote();
        }

        private void LateUpdate() // 여기서 계속 위치를 초기화 해서 카메라가 움직여도 무조건 타겟 위에 이모지가 보이도록 
        {
            if (!target || !mainCamera) return;

            // 카메라 기준 방향 벡터
            Vector3 camForward = (target.position - mainCamera.transform.position).normalized;

            // 카메라의 위쪽 벡터 (시야 기준 위)
            Vector3 camUp = mainCamera.transform.up;

            // 월드 기준이 아닌, 카메라 기준 으로 offset 적용
            Vector3 viewOffset = camUp * offset.y;        // 머리 위로 이동
            Vector3 viewForwardOffset = camForward * offset.z; // 카메라 쪽으로 약간 당기기
            Vector3 viewSideOffset = mainCamera.transform.right * offset.x; // 좌우 미세조정 가능

            // 이모트의 위치를 카메라 기준 위치로 설정 (아무 위치에서든 타겟 위에 이모지가 보이도록)
            transform.position = target.position + viewOffset + viewForwardOffset + viewSideOffset;

            // 카메라를 바라보게 설정 (어느 방향에서든 이모지의 정면(Z축)이 카메라를 바라보도록)
            transform.LookAt(
                transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up
            );
        }

        public void ShowEmote()
        {
            // 이전 Tween 정리
            if (currentSequence != null && currentSequence.IsActive())
                currentSequence.Kill();

            // 초기화
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            transform.localScale = originalScale;
            startPos = target.position + offset;
            transform.position = startPos;

            // 새 Sequence 생성
            currentSequence = DOTween.Sequence();

            // 페이드 인
            currentSequence.Append(spriteRenderer.DOFade(1f, fadeTime).SetEase(Ease.Linear));

            // 유지 (duration초 동안 완전히 표시)
            currentSequence.AppendInterval(duration);

            // 페이드 아웃
            currentSequence.Append(spriteRenderer.DOFade(0f, fadeTime).SetEase(Ease.Linear));

            // 종료 후 초기화
            currentSequence.OnComplete(() =>
            {
                Color color = spriteRenderer.color;
                color.a = 0;
                spriteRenderer.color = color;
                transform.localScale = originalScale;
            });
        }

        private void OnDisable()
        {
            if (currentSequence != null)
                currentSequence.Kill();
        }
    }
}