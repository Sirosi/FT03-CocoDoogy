using UnityEngine;
using DG.Tweening;

/*
(이건 아마 안씀, PageCameraSwiper 사용)
이 코드는 카메라가 세 개의 페이지를 바라보는 (LookAt) 각도를 기반으로 동작합니다.
마우스를 드래그하면 Screen.width를 기준으로 드래그 비율을 계산해서, (0 ~ 1)
특정 비율(snapThreshold) 이상 넘어갔을 때 다음 페이지로 전환되게 했습니다. 

이렇게 하면 PC나 모바일처럼 해상도가 달라도 
항상 화면의 1/4 정도를 드래그해야만 넘어가기 때문에 체감이 동일합니다.

드래그 중에는 Vector3.Slerp를 사용해서 회전이 부드럽게 이어지게 했고,
드래그가 끝나면 DOTween으로 자연스럽게 감속하면서 스냅되도록 만들었습니다.
*/

namespace CocoDoogy.CameraSwiper
{
    public class PageSwiper : MonoBehaviour
    {
        [Header("Page Targets")]
        public Transform[] pages; // Left, Center, Right 순서서

        [Header("Settings")]
        [Tooltip("드래그 감도 (값이 작을수록 느리게 회전)")]
        public float dragSensitivity = 0.001f;

        [Tooltip("스냅 기준 (0.25 = 화면 1/4 드래그 시 전환)")]
        [Range(0f, 1f)] public float snapThreshold = 0.25f;

        [Tooltip("스냅 애니메이션 시간 (초)")]
        public float snapDuration = 0.5f;

        [Tooltip("스냅 전환 이징 (감속 커브)")]
        public Ease snapEase = Ease.OutBack;

        private Vector2 startPos;
        private bool isDragging;
        private int currentPage = 1;   // 현재 페이지 인덱스 (0=Left, 1=Center, 2=Right)
                                       // private int previousPage = -1; // 이전 페이지 인덱스 저장
        private float dragOffset;

        void Start()
        {
            // 시작 시 중앙 페이지만 활성화
            for (int i = 0; i < pages.Length; i++)
                pages[i].gameObject.SetActive(i == currentPage);

            LookAtPageInstant(currentPage);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                float deltaX = (Input.mousePosition.x - startPos.x) / Screen.width;
                dragOffset = Mathf.Clamp(deltaX * dragSensitivity * 1000f, -1f, 1f);
                LookAtDragOffset(dragOffset);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                float normalizedDrag = (Input.mousePosition.x - startPos.x) / Screen.width;
                EvaluateSwipe(normalizedDrag);

                isDragging = false;
                dragOffset = 0f;
            }
        }

        /// <summary>
        /// 반 이상 넘어갔을 때만 페이지 전환
        /// </summary>
        private void EvaluateSwipe(float normalizedDrag)
        {
            // previousPage = currentPage;

            if (Mathf.Abs(normalizedDrag) > snapThreshold)
            {
                if (normalizedDrag > 0) currentPage--;
                else currentPage++;
            }

            currentPage = Mathf.Clamp(currentPage, 0, pages.Length - 1);
            LookAtPageSmooth(currentPage);
        }

        /// <summary>
        /// 드래그 중 실시간으로 페이지 방향 보간
        /// </summary>
        private void LookAtDragOffset(float offset)
        {
            int targetIndex = currentPage;

            if (offset > 0 && currentPage > 0)
                targetIndex = currentPage - 1;
            else if (offset < 0 && currentPage < pages.Length - 1)
                targetIndex = currentPage + 1;

            Vector3 currentDir = (pages[currentPage].position - transform.position).normalized;
            Vector3 targetDir = (pages[targetIndex].position - transform.position).normalized;
            Vector3 blendedDir = Vector3.Slerp(currentDir, targetDir, Mathf.Abs(offset));

            transform.rotation = Quaternion.LookRotation(blendedDir, Vector3.up);
        }

        /// <summary>
        /// 페이지 전환 시 부드럽게 회전 + 이전 페이지 비활성화
        /// </summary>
        private void LookAtPageSmooth(int index)
        {
            // 현재 페이지 활성화
            for (int i = 0; i < pages.Length; i++)
            {
                if (i == index)
                    pages[i].gameObject.SetActive(true);
                else
                    pages[i].gameObject.SetActive(false);
            }
            Vector3 dir = (pages[index].position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

            DOTween.Kill(this);
            transform.DORotateQuaternion(targetRot, snapDuration)
                .SetEase(snapEase)
                .SetId(this)
                .OnComplete(() =>
                {

                });
        }



        /// <summary>
        /// 즉시 해당 페이지 방향으로 회전
        /// </summary>
        private void LookAtPageInstant(int index)
        {
            Vector3 dir = (pages[index].position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}