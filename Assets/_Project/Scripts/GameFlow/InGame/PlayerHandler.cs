using CocoDoogy.Animation;
using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        // 플레이어가 인게임에 들어와서 행동을 했는지 여부
        public static bool IsBehaviour = false;

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


        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<PlayerAnimHandler>();
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
            if (HexTile.GetTile(GridPos)?.HasPiece(PieceType.GravityButton, out _) ?? false)
            {
                preGravityButton = GridPos;
            }

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.transform.position = gridPos.ToWorldPos();
            if (preGravityButton.HasValue) // 실제 기존 발판 리셋하는 곳
            {
                GimmickExecutor.ExecuteFromTrigger(preGravityButton
                    .Value); // Deploy는 갑자기 위치가 바뀌는 문제라 발판이 해결 안 되는 사태를 대비
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
            if (!IsBehaviour) IsBehaviour = true;

            Instance.transform.parent = null;
            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Moving);
            DOTween.Kill(Instance, true);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION)
                .SetId(Instance)
                .OnComplete(() =>
                {
                    OnBehaviourCompleted();
                    HexTile currentTile = HexTile.GetTile(gridPos);
                    if (currentTile != null && currentTile.CurrentData.stepSfx != SfxType.None)
                    {
                        SfxManager.PlaySfx(currentTile.CurrentData.stepSfx);
                    }
                });
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
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(Instance)
                .OnComplete(OnBehaviourCompleted);
        }


        private static void OnBehaviourCompleted()
        {
            Instance.anim.ChangeAnim(AnimType.Idle);
        }
    }
}