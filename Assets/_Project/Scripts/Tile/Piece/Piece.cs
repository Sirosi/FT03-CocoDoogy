using CocoDoogy.Data;
using CocoDoogy.LifeCycle;
using Lean.Pool;
using System;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    public class Piece: MonoBehaviour
    {
        public PieceData BaseData { get; private set; } = null;
        public HexTile Parent { get; private set; } = null;
        public HexDirection DirectionPos { get; private set; } = HexDirection.East;
        /// <summary>
        /// 현재 Piece가 Center Piece라면, 바라보는 방향
        /// </summary>
        public HexDirection LookDirection { get; set; } = HexDirection.East;
        public Vector2Int? Target { get; set; } = null;
        /// <summary>
        /// 특수한 기물을 위한 데이터
        /// </summary>
        public string SpecialData
        {
            get => specialData;
            set
            {
                specialData = value;
                onDataInsert?.Invoke(specialData);
            }
        }

        public bool IsTrigger => BaseData && BaseData.type is PieceType.Switch or PieceType.Button or PieceType.GravityButton;
        
        
        /// <summary>
        /// 부모가 정해졌을 때 동작
        /// </summary>
        private Action<Piece> onSpawn = null;
        /// <summary>
        /// 릴리즈시 동작
        /// </summary>
        private Action<Piece> onRelease = null;
        
        /// <summary>
        /// 데이터 입력
        /// </summary>
        private Action<string> onDataInsert = null;
        /// <summary>
        /// 특수행동
        /// </summary>
        private Action onExecute = null;

        private bool hasInit = false;
        private string specialData = string.Empty;
        
        
        #region ◇ LifeCycle ◇
        private void Init(Piece data)
        {
            hasInit = true;
            GetComponentsInChildren<IInit<Piece>>().GetEvents()?.Invoke(data);
            
            onSpawn = GetComponentsInChildren<ISpawn<Piece>>().GetEvents();
            onRelease = GetComponentsInChildren<IRelease<Piece>>().GetEvents();
            onDataInsert = GetComponentsInChildren<ISpecialPiece>().GetInserts();
            onExecute = GetComponentsInChildren<ISpecialPiece>().GetExecutes();
        }

        private void Spawn(PieceData data)
        {
            BaseData = data;
        }
        public void Release()
        {
            int idx = (int)DirectionPos;
            
            Parent.Pieces[idx] = null;
            Parent = null;
            Target = null;
            
            onRelease?.Invoke(this);
            LeanPool.Despawn(this);
        }
        #endregion


        public void SetPosition(HexDirection direction)
        {
            transform.localPosition = (DirectionPos = direction).GetPos();

            if (DirectionPos == HexDirection.Center) // Center Piece는 바라보는 방향 계산식이 다름
            {
                HexRotate rotate = (HexRotate)LookDirection;
                transform.rotation = Quaternion.Euler(0, -(int)rotate * 60, 0); // 120을 더하는 이유는 NW 모서리가 기본적으로 120도 돌아가야하기 때문
            }
            else
            {
                transform.LookAt(Parent.transform, Vector3.up);
            }
        }
        
        public void SetParent(HexTile parent)
        {
            transform.parent = parent.PieceGroup;
            Parent = parent;
            
            onSpawn?.Invoke(this);
        }

        public void Execute()
        {
            onExecute?.Invoke();
        }
        
        
        public static Piece Create(PieceType pieceType, HexTile parent)
        {
            PieceData data = DataManager.GetPieceData(pieceType);
            
            return data is null ? null : Create(data, parent);
        }
        public static Piece Create(PieceData data, HexTile parent)
        {
            Piece result = LeanPool.Spawn(data.modelPrefab, parent.PieceGroup);
            if (!result.hasInit)
            {
                // Component 연결 등의 초기화를 한 적이 없는 생성된 타일이라면, 초기화
                result.Init(result);
            }
            result.Spawn(data);

            return result;
        }
    }
}