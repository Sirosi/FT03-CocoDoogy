using CocoDoogy.Animation;
using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Utility;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class PlayerHandler: Singleton<PlayerHandler>
    {
        public static Vector2Int GridPos
        {
            get => Instance?.gridPos ?? Vector2Int.zero;
            set
            {
                if (!IsValid) return;

                Instance.gridPos = value;
            }
        }

        public static HexDirection LookDirection
        {
            get => Instance?.lookDirection ?? HexDirection.East;
            set
            {
                if (!IsValid) return;
                if (Instance.lookDirection == value) return;

                Instance.lookDirection = value;
                Instance.transform.DORotate(new Vector3(0, value.ToDegree(), 0), Constants.MOVE_DURATION);
            }
        }


        /// <summary>
        /// 현재 플레이어가 동작할 수 있을지 판단
        /// </summary>
        private static bool IsValid
        {
            get
            {
                if (!Instance) return false;

                return true;
            }
        }


        private Vector2Int gridPos = Vector2Int.zero;
        private HexDirection lookDirection = HexDirection.East;
        private PlayerAnimHandler anim = null;

        private Camera mainCamera = null;
        private bool touched = false;
        private HexTile pointDownTile = null;


        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<PlayerAnimHandler>();
        }
        void Start()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (TouchSystem.IsPointerOverUI) return;
            
            if (TouchSystem.TouchCount > 0)
            {
                if (touched) return;
                
                touched = true;
                Ray ray = mainCamera.ScreenPointToRay(TouchSystem.TouchAverage);
                pointDownTile = GetRayTile(ray);
            }
            else
            {
                if (!touched) return;
                
                touched = false;
                if (!pointDownTile) return;
                
                Ray ray = mainCamera.ScreenPointToRay(TouchSystem.TouchAverage);
                HexTile pointUpTile = GetRayTile(ray);
                if (pointDownTile != pointUpTile) return;
                if (!pointDownTile) return;
                
                HexDirection? direction = GridPos.GetRelativeDirection(pointUpTile.GridPos);
                if (!direction.HasValue) return;

                HexTile playerTile = HexTile.GetTile(GridPos);
                if (!playerTile.CanMove(direction.Value)) return;

                CommandManager.Move(direction.Value);
            }
        }
        /// <summary>
        /// Ray에 부딪힌 HexTile을 반환
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        private HexTile GetRayTile(Ray ray)
        {
            HexTile result = null;
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Tile")))
            {
                result = hit.collider.GetComponentInParent<HexTile>();                ;
            }
            return result;
        }


        public static void Clear()
        {
            if (!IsValid) return;
        }


        /// <summary>
        /// 순간이동 등의 갑작스래 위치가 변경
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Deploy(Vector2Int gridPos)
        {
            if (!IsValid) return;

            // 추후 Move 및 Slide에서 사용할지 고민 좀 해봐야할 듯 함
            Vector2Int? preGravityButton = null;
            if(HexTile.GetTile(GridPos)?.HasPiece(PieceType.GravityButton, out _) ?? false)
            {
                preGravityButton = GridPos;
            }

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.transform.position = gridPos.ToWorldPos();
            if(preGravityButton.HasValue) // 실제 기존 발판 리셋하는 곳
            {
                GimmickExecutor.ExecuteFromTrigger(preGravityButton.Value); // Deploy는 갑자기 위치가 바뀌는 문제라 발판이 해결 안 되는 사태를 대비
            }
            OnBehaviourCompleted();
        }

        /// <summary>
        /// 보행으로 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Move(Vector2Int gridPos)
        {
            if (!IsValid) return;

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Moving);
            DOTween.Kill(Instance, true);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION)
                .SetId(Instance)
                .OnComplete(OnMoveComplete);
        }
        private static void OnMoveComplete()
        {
            OnBehaviourCompleted();
            HexTile currentTile = HexTile.GetTile(GridPos);
            if (currentTile != null && currentTile.CurrentData.stepSfx != SfxType.None)
            {
                SfxManager.PlaySfx(currentTile.CurrentData.stepSfx);
            }
        }
        
        /// <summary>
        /// 미끄러지듯 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Slide(Vector2Int gridPos)
        {
            if (!IsValid) return;

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Slide);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(Instance).OnComplete(OnBehaviourCompleted);
        }


        private static void OnBehaviourCompleted()
        {
            Instance.anim.ChangeAnim(AnimType.Idle);
        }
    }
}