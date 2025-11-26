using UnityEngine;
using UnityEngine.InputSystem;

namespace CocoDoogy.TPSView
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public float gravity = -9.81f;

        private CharacterController controller;
        private Vector3 velocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            HandleMovement();
            ApplyGravity();
        }

        void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(h) < 0.001f && Mathf.Abs(v) < 0.001f)
                return;

            // 카메라 기준으로 입력 방향 변환
            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Vector3 camForward = mainCam.transform.forward;
            Vector3 camRight = mainCam.transform.right;

            // Y축 제거 (수평면에서만 이동)
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // 카메라 기준 이동 방향 계산
            Vector3 moveDir = (camForward * v + camRight * h).normalized;

            // 이동 방향으로 회전
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            // 이동
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        void ApplyGravity()
        {
            if (controller.isGrounded)
                velocity.y = -2f;
            else
                velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
