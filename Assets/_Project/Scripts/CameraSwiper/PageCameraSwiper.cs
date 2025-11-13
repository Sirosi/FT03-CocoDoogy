using UnityEngine;
using DG.Tweening;

/*
PageCameraSwiper는 드래그 방향에 따라 카메라가 페이지 간을 부드럽게 이동하는 시스템입니다.
드래그 시 카메라는 현재 페이지와 다음 페이지 사이의 위치·회전 중간값으로 실시간 이동하며,
일정 비율(snapThreshold) 이상 드래그 시 다음 페이지로 스무스하게 전환됩니다.

전환 시에는 새 페이지가 바로 활성화되어 페이드 인 효과로 자연스럽게 등장하고,
이전 페이지는 전환 완료 후 자동 비활성화됩니다.
Ease.OutBack 곡선을 사용하여 살짝 튕기듯 감속되는 부드러운 전환감을 구현했습니다.
*/

namespace CocoDoogy.CameraSwiper
{
    public class PageCameraSwiper : MonoBehaviour
    {
        [Header("Camera Points")]
        [SerializeField] private Transform[] cameraPoints; // Left, Center, Right 등 페이지 위치
        private int currentIndex = 1; // 중앙에서 시작 (0=Left, 1=Center, 2=Right)

        [Header("Pages")]
        [SerializeField] private GameObject[] pages;

        [Header("Settings")]
        [Tooltip("드래그 감도")]
        [SerializeField] private float dragSensitivity = 0.25f;

        [Tooltip("페이지 전환 임계값 (0~1, 화면 폭 대비 비율)")]
        [Range(0f, 1f)] public float snapThreshold = 0.25f;

        [Tooltip("전환 속도 (초)")]
        [SerializeField] private float snapDuration = 0.5f;

        [Tooltip("전환 곡선 (살짝 튕기는 감속 효과)")]
        [SerializeField] private Ease snapEase = Ease.OutBack;

        [Tooltip("전환 강도")]
        [SerializeField] private float snapForce = 0.75f;

        private Vector2 startPos; // 드래그 시작 위치
        private bool isDragging; // 드래그 중인지 여부
        private float dragPercent; // 드래그 비율
        private int targetIndex; // 타겟 인덱스
        private float lerpSpeed = 12f; // 카메라 실시간 보간 속도

        void Start()
        {
            MoveToPageInstant(currentIndex);
            SetActivePage(currentIndex); // 초기 페이지 활성화
        }

        void Update()
        {
            // 마우스 좌우 드래그 시 카메라 이동 시작
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                isDragging = true;
            }

            // 드래그 중 실시간 이동 (Lerp로 구현 - 부드럽게 이동) 마우스 좌우 드래그 중 카메라 이동
            if (Input.GetMouseButton(0) && isDragging)
            {
                float deltaX = (Input.mousePosition.x - startPos.x) / Screen.width;
                dragPercent = Mathf.Clamp(deltaX * dragSensitivity, -1f, 1f);

                targetIndex = currentIndex;
                if (dragPercent > 0 && currentIndex > 0) // 왼쪽으로 드래그 인덱스 0이면 작동안함(왼쪽끝)
                    targetIndex = currentIndex - 1;
                else if (dragPercent < 0 && currentIndex < cameraPoints.Length - 1) // 오른쪽으로 드래그 인덱스 2이면 작동안함(오른쪽끝)
                    targetIndex = currentIndex + 1;

                Vector3 blendedPos = Vector3.Lerp(
                    cameraPoints[currentIndex].position,
                    cameraPoints[targetIndex].position,
                    Mathf.Abs(dragPercent)
                );

                Quaternion blendedRot = Quaternion.Slerp(
                    cameraPoints[currentIndex].rotation,
                    cameraPoints[targetIndex].rotation,
                    Mathf.Abs(dragPercent)
                );

                transform.position = Vector3.Lerp(transform.position, blendedPos, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, blendedRot, Time.deltaTime * lerpSpeed);
            }

            // 드래그 종료 후 비율검사 함수 호출 (EvaluateSwipe)
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                float normalizedDrag = (Input.mousePosition.x - startPos.x) / Screen.width;
                EvaluateSwipe(normalizedDrag);
            }
        }

        // 드래그 비율 이상 넘어가는거 검사 후 바로 스무스한 전환 처리
        private void EvaluateSwipe(float normalizedDrag)
        {
            if (Mathf.Abs(normalizedDrag) > snapThreshold)
            {
                if (normalizedDrag > 0) currentIndex--;
                else currentIndex++;
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, cameraPoints.Length - 1);
            MoveToPageSmooth(currentIndex);
        }


        // 바로 전환은 (DOTween으로 OutBack으로 마지막 약간의 튕김 효과 추가)
        // 카메라 이동 및 회전
        private void MoveToPageSmooth(int index)
        {
            DOTween.Kill(this);

            // 전환 시작 시 현재 + 다음 페이지 활성화
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == null) continue;
                bool shouldBeActive = (i == currentIndex) || (i == index);
                pages[i].SetActive(shouldBeActive);
            }

            // 카메라 이동 및 회전
            transform.DOMove(cameraPoints[index].position, snapDuration)
                .SetEase(Ease.OutBack, snapForce) // 살짝만 튕김
                .SetId(this);

            transform.DORotateQuaternion(cameraPoints[index].rotation, snapDuration)
                .SetEase(Ease.OutBack, snapForce)
                .SetId(this)
                .OnComplete(() =>
                {
                    // 완료 시 해당 페이지만 남기고 나머지는 비활성화
                    SetActivePage(index);
                });
        }

        // 처음 기본 페이지 초기화용 (center 페이지)
        private void MoveToPageInstant(int index)
        {
            transform.position = cameraPoints[index].position;
            transform.rotation = cameraPoints[index].rotation;
        }

        // 활성화된 페이지 검사 후 페이지 활성화 처리
        private void SetActivePage(int activeIndex)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == null) continue;
                pages[i].SetActive(i == activeIndex); // 인덱스 검사해서 현재 페이지만 true
            }
        }

        void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}
