using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class GimmickCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        public Vector2Int GridPos = Vector2Int.zero;
        public GimmickType Gimmick = GimmickType.None;
        public int Data = 0;
        
        
        public GimmickCommand(object param) : base(CommandType.Gimmick, param)
        {
            var data = ((Vector2Int, GimmickType, int))param;
            GridPos = data.Item1;
            Gimmick = data.Item2;
            Data = data.Item3;
        }

        
        public override void Execute()
        {
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    
                    break;
                case GimmickType.PieceChange:
                    break;
                case GimmickType.PieceDestroy:
                    break;
            }
        }

        public override void Undo()
        {
            
        }
    }
}