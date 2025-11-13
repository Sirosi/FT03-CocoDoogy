using CocoDoogy.Animation;
using CocoDoogy.Core;
using CocoDoogy.Tile;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class PlayerHandler: Singleton<PlayerHandler>
    {
        private static readonly Stack<HexTile> filledTiles = new();

        public static Vector2Int GridPos
        {
            get => Instance?.gridPos ?? Vector2Int.zero;
            set
            {
                if (!IsValid) return;

                Instance.gridPos = value;

                while (filledTiles.Count > 0)
                {
                    filledTiles.Pop().OffOutline();
                }

                Vector2Int gridPos = Instance.gridPos = value;
                HexTile tile = HexTile.GetTile(gridPos);
                List<Vector2Int> canPoses = tile.CanMovePoses();
                // 갈 수 있는 타일 색칠
                foreach (var canPos in canPoses)
                {
                    HexTile canTile = HexTile.GetTile(canPos);
                    if (!canTile) continue;

                    canTile.DrawOutline(Color.green);
                    filledTiles.Push(canTile);
                }
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

            filledTiles.Clear();
        }


        /// <summary>
        /// 순간이동 등의 갑작스래 위치가 변경
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Deploy(Vector2Int gridPos)
        {
            if (!IsValid) return;

            DOTween.Kill(Instance, true);
            GridPos = gridPos;
            Instance.transform.position = gridPos.ToWorldPos();
            OnBehaviourCompleted();
        }

        /// <summary>
        /// 보행으로 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Move(Vector2Int gridPos)
        {
            if (!IsValid) return;

            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Moving);
            DOTween.Kill(Instance, true);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(Instance).OnStepComplete(OnBehaviourCompleted);
        }
        /// <summary>
        /// 미끄러지듯 이동
        /// </summary>
        /// <param name="gridPos"></param>
        public static void Slide(Vector2Int gridPos)
        {
            if (!IsValid) return;

            GridPos = gridPos;
            Instance.anim.ChangeAnim(AnimType.Slide);
            DOTween.Kill(Instance, true);
            Instance.transform.DOMove(gridPos.ToWorldPos(), Constants.MOVE_DURATION).SetId(Instance).OnStepComplete(OnBehaviourCompleted);
        }


        private static void OnBehaviourCompleted()
        {
            Instance.anim.ChangeAnim(AnimType.Idle);
        }
    }
}