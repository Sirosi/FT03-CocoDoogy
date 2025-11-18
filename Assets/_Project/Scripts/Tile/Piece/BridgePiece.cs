using CocoDoogy.GameFlow.InGame;
using CocoDoogy.LifeCycle;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// 다리용
    /// </summary>
    public class BridgePiece: MonoBehaviour, ISpawn<Piece>, IRelease<Piece>
    {
        private HexDirection LookDirection => piece.LookDirection;


        /// <summary>
        /// 이 기물이 올라간 타일
        /// </summary>
        private HexTile Parent
        {
            get => parent;
            set
            {
                if (parent)
                {
                    parent.OnRotateComplete -= OnParentRotateComplete;
                }
                (parent = value).OnRotateComplete += OnParentRotateComplete;
            }
        }
        
        
        private HexTile parent = null;
        private Piece piece = null;

        private HexTile frontTile = null;
        private HexTile backTile = null;
        
        
        public void OnSpawn(Piece piece)
        {
            Parent = (this.piece = piece).Parent;
            ConnectSideTile();
        }
        public void OnRelease(Piece data)
        {
            if (Parent)
            {
                Parent.OnRotateComplete -= OnParentRotateComplete;
            }
            DisconnectEvents();
        }
        

        private void ConnectSideTile()
        {
            DisconnectEvents();
            
            Vector2Int frontPos = Parent.GridPos.GetDirectionPos(LookDirection);
            Vector2Int backPos = Parent.GridPos.GetDirectionPos(LookDirection.GetMirror());

            frontTile = HexTile.GetTile(frontPos);
            backTile = HexTile.GetTile(backPos);

            ConnectEvents();
        }


        private void OnParentRotateComplete(HexTile tile, HexRotate rotate)
        {
            ConnectSideTile();
        }
        private void OnRotated(HexTile tile, HexRotate rotate)
        {
            Vector3 prePos = piece.transform.position;
            Quaternion preRot = piece.transform.rotation;
            piece.LookDirection = LookDirection.AddRotate(rotate);
            Parent.Pieces[(int)HexDirection.Center] = null;
            
            HexDirection directionOfRotateTile = tile == frontTile ? LookDirection.GetMirror() : LookDirection;
            Vector2Int nextParentPos = tile.GridPos.GetDirectionPos(directionOfRotateTile);
            HexTile nextParent = HexTile.GetTile(nextParentPos);
            if (nextParent)
            {
                Parent = HexTile.GetTile(nextParentPos);
                Parent.ConnectPiece(HexDirection.Center, piece);
                piece.transform.position = prePos;
                piece.transform.rotation = preRot;
                piece.transform.parent = tile.transform;
            }
            else
            {
                MessageDialog.ShowMessage("에러 발생", "타일이 없는 곳으로 다리가 회전함", DialogMode.Confirm, null);
                piece.Release();
            }
        }
        private void OnRotateComplete(HexTile tile, HexRotate rotate)
        {
            if (!Parent)
            {
                Debug.LogError("갈 타일이 없다.");
                return;
            }
            Parent.ConnectPiece(HexDirection.Center, piece);
            
            ConnectSideTile();
        }
        
        private void ConnectEvents()
        {
            if (frontTile)
            {
                frontTile.OnRotateChanged += OnRotated;
                frontTile.OnRotateComplete += OnRotateComplete;
            }
            if (backTile)
            {
                backTile.OnRotateChanged += OnRotated;
                backTile.OnRotateComplete += OnRotateComplete;
            }
        }
        private void DisconnectEvents()
        {
            if (frontTile)
            {
                frontTile.OnRotateChanged -= OnRotated;
                frontTile.OnRotateComplete -= OnRotateComplete;
            }
            if (backTile)
            {
                backTile.OnRotateChanged -= OnRotated;
                backTile.OnRotateComplete -= OnRotateComplete;
            }
        }
    }
}