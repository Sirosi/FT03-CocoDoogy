using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class SailCommand: CommandBase
    {
        public override bool IsUserCommand => true;


        /// <summary>
        /// 이전 위치
        /// </summary>
        public Vector2Int PrePos = Vector2Int.zero;

        /// <summary>
        /// 다음 위치
        /// </summary>
        public Vector2Int NextPos = Vector2Int.zero;
        
        
        public SailCommand(object param) : base(CommandType.Sail, param)
        {
            var data = ((Vector2Int, Vector2Int))param;
            PrePos = data.Item1;
            NextPos = data.Item2;
        }


        public override void Execute()
        {
            Debug.Log($"{NextPos} - {PrePos}");
            PlayerHandler.Deploy(NextPos);
            InGameManager.ConsumeActionPoint(1);
            
            // 출발지 정리
            HexTile tile = HexTile.GetTile(PrePos);
            tile.HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = false;
                
            // 도착지 정리
            tile = HexTile.GetTile(NextPos);
            if (tile.HasPiece(PieceType.Deck, out Piece destoPiece))
            {
                destoPiece.GetComponent<DeckPiece>().IsDocked = true;
            }
        }

        public override void Undo()
        {
            Debug.Log($"{NextPos} - {PrePos}");
            PlayerHandler.Deploy(PrePos);
            InGameManager.RegenActionPoint(1);
            
            // 출발지 정리
            HexTile tile = HexTile.GetTile(NextPos);
            if (tile.HasPiece(PieceType.Deck, out Piece destoPiece))
            {
                destoPiece.GetComponent<DeckPiece>().IsDocked = false;
            }
                
            // 도착지 정리
            tile = HexTile.GetTile(PrePos);
            tile.HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = true;
        }
    }
}