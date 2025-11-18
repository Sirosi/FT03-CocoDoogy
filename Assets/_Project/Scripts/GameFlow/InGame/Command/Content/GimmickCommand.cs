using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 기믹과 관련된 처리를 하는 Command<br/>
    /// GridPos(Vector2Int), Gimmick(GimmickType), MainData(int), SubData(int), PreSubData(int), Dir(HexDirection), PreDIr(HexDirection), DidGimmick(bool)<br/>
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
        public HexDirection Dir = 0;
        public HexDirection PreDir = 0;
        public bool DidGimmick = false;
        
        
        public GimmickCommand(object param) : base(CommandType.Gimmick, param)
        {
            var data = ((Vector2Int, GimmickType, int, int, int, HexDirection, HexDirection, bool))param;
            GridPos = data.Item1;
            Gimmick = data.Item2;
            MainData = data.Item3;
            SubData = data.Item4;
            PreSubData = data.Item5;
            Dir = data.Item6;
            PreDir = data.Item7;
            DidGimmick = data.Item8;
        }

        
        public override void Execute()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            GimmickData gimmick = HexTileMap.GetGimmick(GridPos);
            if (DidGimmick && gimmick != null)
            {
                gimmick.IsOn = !gimmick.IsOn;
            }
                
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)MainData);
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)SubData, Dir);
                    break;
                case GimmickType.PieceMove:
                    tile.GetPiece((HexDirection)MainData).Move(Dir);
                    break;
            }
        }

        public override void Undo()
        {
            HexTile tile = HexTile.GetTile(GridPos);
            GimmickData gimmick = HexTileMap.GetGimmick(GridPos);
            if (DidGimmick && gimmick != null)
            {
                gimmick.IsOn = !gimmick.IsOn;
            }
            
            switch (Gimmick)
            {
                case GimmickType.TileRotate:
                    tile.Rotate((HexRotate)(-MainData));
                    break;
                case GimmickType.PieceChange:
                    tile.SetPiece((HexDirection)MainData, (PieceType)PreSubData, PreDir);
                    break;
                case GimmickType.PieceMove:
                    tile = HexTile.GetTile(GridPos.GetDirectionPos(Dir));
                    tile.GetPiece((HexDirection)MainData).Move(PreDir);
                    break;
            }
        }
    }
}