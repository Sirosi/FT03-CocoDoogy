using System;

namespace CocoDoogy.Tile.Piece
{
    [Flags]
    public enum PiecePosType
    {
        None    = 0,
        Side    = 1 << 0,
        Center  = 1 << 1,
    }
}