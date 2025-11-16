using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 기믹과 관련된 처리를 하는 Command<br/>
    /// GridPos(Vector2Int), Gimmick(GimmickType), MainData(int), SubData(int), PreSubData(int), LookDirection(HexDirection)<br/>
    /// 
    /// </summary>
    public class GimmickCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        public Vector2Int GridPos = Vector2Int.zero;
        public GimmickType Gimmick = GimmickType.None;
        public int MainData = 0;
        public int SubData = 0;
        public int PreSubData = 0;
        public HexDirection LookDirection = 0;
        
        
        public GimmickCommand(object param) : base(CommandType.Gimmick, param)
        {
            var data = ((Vector2Int, GimmickType, int, int, int, HexDirection))param;
            GridPos = data.Item1;
            Gimmick = data.Item2;
            MainData = data.Item3;
            SubData = data.Item4;
            PreSubData = data.Item5;
            LookDirection = data.Item6;
        }

        
        public override void Execute()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)MainData);
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)SubData, LookDirection);
                    break;
            }
        }

        public override void Undo()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)(-MainData));
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)PreSubData, LookDirection);
                    break;
            }
        }
    }
}