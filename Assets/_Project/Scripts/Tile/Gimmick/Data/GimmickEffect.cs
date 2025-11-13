using CocoDoogy.Tile.Piece;

namespace CocoDoogy.Tile.Gimmick.Data
{
    [System.Serializable]
    public class GimmickEffect
    {
        public HexRotate Rotate = HexRotate.None;
        public HexDirection Direction = HexDirection.East;
        public HexDirection LookDirection = HexDirection.East;
        public PieceType PieceType = PieceType.None;
    }
}