using CocoDoogy.Tile;
using CocoDoogy.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.TPSView
{
    public class InGameCameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float zoomSpeed = 3f;


        void Awake()
        {
            MapSaveLoader.OnMapLoaded += InitCameraPos;
        }

        void OnDestroy()
        {
            MapSaveLoader.OnMapLoaded -= InitCameraPos;
        }

        void Update()
        {
            if (IsPointerOverUI()) return;

            Move();
            Zoom();
        }


        private void InitCameraPos()
        {
            cameraPivot.transform.position = (HexTileMap.MaxPoint + HexTileMap.MinPoint) * 0.5f;
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
            //==================
            // Touch Began
            //==================
            if(TouchSystem.TouchCount > 0 && lastTouchcount != TouchSystem.TouchCount)
            {
                hasMoving = true;
                lastTouchcount = TouchSystem.TouchCount;
                prevPos = TouchSystem.TouchAverage;
                return;
            }
            //==================
            // Touch Ended
            //==================
            else if(hasMoving && TouchSystem.TouchCount <= 0)
            {
                hasMoving = false;
                lastTouchcount = TouchSystem.TouchCount;
                return;
            }


            //==================
            // Touch ing
            //==================
            if(!hasMoving) return;
            Vector2 currentPos = TouchSystem.TouchAverage;
            Vector2 delta = currentPos - prevPos;
            prevPos = currentPos;

            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * (moveSpeed * Time.deltaTime);
            cameraPivot.transform.Translate(move, Space.World);

            // 카메라 최대크기 제약
            cameraPivot.transform.position = Vector3.Min(cameraPivot.transform.position, HexTileMap.MaxPoint);
            cameraPivot.transform.position = Vector3.Max(cameraPivot.transform.position, HexTileMap.MinPoint);
        }
        #endregion
        
        #region ◇ Zoom ◇
        private bool hasZooming = false;
        private float preDistance = 0f;
        private void Zoom()
        {
            //==================
            // Mouse Zoom
            //==================
            if(TouchSystem.CurrentInputType == TouchSystem.InputType.Mouse) // Mouse Scroll
            {
                float scroll = Mouse.current.scroll.ReadValue().y;

                if (Mathf.Abs(scroll) > 0.1f)
                {
                    mainCamera.fieldOfView -= scroll * zoomSpeed;
                    mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
                }
                return;
            }


            //==================
            // TouchScreen Zoom
            //==================
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

            mainCamera.fieldOfView -= delta * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20, 80);
        }
        #endregion
    }
}