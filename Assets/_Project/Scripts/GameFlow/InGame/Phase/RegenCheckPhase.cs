using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    // TODO: 임시 Phase
    public class RegenCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            Piece centerPiece = HexTile.GetTile(PlayerHandler.GridPos)?.GetPiece(HexDirection.Center);
            if (!centerPiece || centerPiece.BaseData.type is not (PieceType.Field or PieceType.House)) return true;
            
            CommandManager.Regen(centerPiece.BaseData.type == PieceType.Field ? 1 : 2);
            CommandManager.GimmickPieceChange(PlayerHandler.GridPos, HexDirection.Center, PieceType.None,
                centerPiece.BaseData.type, centerPiece.LookDirection, centerPiece.LookDirection);
            
            return true;
        }
    }
}