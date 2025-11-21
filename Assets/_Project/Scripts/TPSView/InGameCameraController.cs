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


        private void Start()
        {
            if (!mainCamera) return;
            mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (IsPointerOverUI()) return;

            Move();
            Zoom();
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

        #region ◇ Move ◇
        private bool hasMoving = false;
        private Vector2 prevPos = Vector2.zero;
        private int lastTouchcount = 0;
        private void Move()
        {
            if(TouchSystem.TouchCount > 0 && lastTouchcount != TouchSystem.TouchCount) // Touch Began
            {
                hasMoving = true;
                lastTouchcount = TouchSystem.TouchCount;
                prevPos = TouchSystem.TouchAverage;
                return;
            }
            else if(hasMoving && TouchSystem.TouchCount <= 0) // Touch Ended
            {
                hasMoving = false;
                lastTouchcount = TouchSystem.TouchCount;
                return;
            }


            if(!hasMoving) return;
            Vector2 currentPos = TouchSystem.TouchAverage;
            Vector2 delta = currentPos - prevPos;
            prevPos = currentPos;

            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * (moveSpeed * Time.deltaTime);
            mainCamera.transform.Translate(move, Space.World);
        }
        #endregion
        
        #region ◇ Zoom ◇
        private bool hasZooming = false;
        private float preDistance = 0f;
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
                preDistance = TouchSystem.DistanceAverage;
                return;
            }
            else if(hasZooming && TouchSystem.TouchCount < 2) // Touch Ended
            {
                hasZooming = false;
                return;
            }

            if(!hasZooming) return;
            float currentDistance = TouchSystem.DistanceAverage;
            float delta = currentDistance - preDistance;
            preDistance = currentDistance;

            mainCamera.fieldOfView -= delta / Screen.height * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
        }
        #endregion
    }
}