using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// Player와 Crate가 같은 타일로 있으면 이동 시도
    /// </summary>
    public class CrateMovePhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile) return false; // 타일이 없는 건 경우가 다른 사태임
            
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            if (!centerPiece || centerPiece.BaseData.type != PieceType.Crate) return true; // 중앙 기물이 없거나 상자가 아님
            
            Vector2Int gridPos = PlayerHandler.GridPos;
            HexDirection moveDir = PlayerHandler.LookDirection;
            CommandManager.GimmickPieceMove(gridPos, HexDirection.Center, moveDir);

            return true;
        }
    }
}