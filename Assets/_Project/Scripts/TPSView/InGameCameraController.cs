using CocoDoogy.Tile;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace CocoDoogy
{
    public class InGameCameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float zoomSpeed = 3f;
        
        private Vector2 prevPos;
        private bool canMoveCamera = false;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (Touchscreen.current != null)
            {
                HandleTouch();
                HandleTouchZoom();
            }
            else
            {
                HandleMouse();
                HandleMouseZoom();
            }
        }

        private void HandleMouse()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                canMoveCamera = !ObjectCheck(Mouse.current.position.ReadValue());
                prevPos = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.leftButton.isPressed && canMoveCamera)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();
                Vector2 delta = currentPos - prevPos;
                prevPos = currentPos;
                MoveCamera(delta);
            }
        }

        private void HandleTouch()
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                canMoveCamera = !ObjectCheck(touch.position.ReadValue());
                prevPos = touch.position.ReadValue();
            }

            if (touch.press.isPressed && canMoveCamera)
            {
                Vector2 currentPos = touch.position.ReadValue();
                Vector2 delta = currentPos - prevPos;
                prevPos = currentPos;

                MoveCamera(delta);
            }
        }
        private bool ObjectCheck(Vector2 screenPos)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                return true; // 오브젝트 있음
            }

            return false; // 오브젝트 없음 → 카메라 이동 가능
        }
        
        private void MoveCamera(Vector2 delta)
        {
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * (moveSpeed * Time.deltaTime);
            mainCamera.transform.Translate(move, Space.World);
        }
        
        
        private void HandleMouseZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) > 0.1f)
            {
                mainCamera.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
            }
        }

        private void HandleTouchZoom()
        {
            if (Touchscreen.current == null) return;

            var touches = Touchscreen.current.touches;
            int count = 0;
            foreach (var t in touches)
                if (t.press.isPressed) count++;

            if (count != 2) return; 
            
            TouchControl t1 = touches[0];
            TouchControl t2 = touches[1];

            if (!t1.press.isPressed || !t2.press.isPressed) return;
            
            Vector2 prevPos1 = t1.position.ReadValue() - t1.delta.ReadValue();
            Vector2 prevPos2 = t2.position.ReadValue() - t2.delta.ReadValue();
            float prevDistance = Vector2.Distance(prevPos1, prevPos2);

            Vector2 currPos1 = t1.position.ReadValue();
            Vector2 currPos2 = t2.position.ReadValue();
            float currDistance = Vector2.Distance(currPos1, currPos2);

            float diff = currDistance - prevDistance;

            mainCamera.fieldOfView -= diff * zoomSpeed * Time.deltaTime;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
        }
    }
}