using CocoDoogy.Tile;
using CocoDoogy.Utility;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (IsPointerOverUI()) return;

            Move();
            Zoom();
        }

        private bool hasMoving = false;
        private void Move()
        {
            if(!hasMoving && TouchSystem.TouchCount == 1) // Touch Began
            {
                hasMoving = true;
                prevPos = TouchSystem.TouchAverage;
                return;
            }
            else if(hasMoving && TouchSystem.TouchCount <= 0) // Touch Ended
            {
                hasMoving = false;
                return;
            }


            if(!hasMoving) return;
            Vector2 currentPos = TouchSystem.TouchAverage;
            Vector2 delta = currentPos - prevPos;
            prevPos = currentPos;

            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * (moveSpeed * Time.deltaTime);
            mainCamera.transform.Translate(move, Space.World);
        }
        
        private bool hasZooming = false;
        private float pivotDistance = 0f;
        private void Zoom()
        {
            if(TouchSystem.CurrentInputType == TouchSystem.InputType.Mouse) // Mouse Scroll
            {
                float scroll = Mouse.current.scroll.ReadValue().y;

                if (Mathf.Abs(scroll) > 0.1f)
                {
                    mainCamera.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
                    mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
                }
                return;
            }


            if(!hasZooming && TouchSystem.TouchCount >= 2) // Touch Began
            {
                hasZooming = true;
                pivotDistance = TouchSystem.DistanceAverage;
                return;
            }
            else if(hasZooming && TouchSystem.TouchCount < 2) // Touch Ended
            {
                hasZooming = false;
                return;
            }

            if(!hasZooming) return;
            float currentDistance = TouchSystem.DistanceAverage;
            float distance = (currentDistance - pivotDistance) / pivotDistance;

            mainCamera.fieldOfView -= distance * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
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
        
        private bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            if (Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return true;
            }

            return false;
        }
    }
}