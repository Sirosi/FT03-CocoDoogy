using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 부두의 배를 되돌려 놓는 Command<br/>
    /// GridPos(Vector2Int), Docked(bool)
    public class DeckResetCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        [SerializeField] private Vector2Int gp = Vector2Int.zero;
        [SerializeField] private bool d = false;
        

        public Vector2Int GridPos { get => gp; private set => gp = value; }
        public bool Docked { get => d; private set => d = value; }


        public DeckResetCommand(object param): base(CommandType.DeckReset, param)
        {
            var data = ((Vector2Int, bool))param;

            GridPos = data.Item1;
            Docked = data.Item2;
        }


        public override void Execute()
        {
            HexTile.GetTile(GridPos).HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = Docked;
        }

        public override void Undo()
        {
            HexTile.GetTile(GridPos).HasPiece(PieceType.Deck, out Piece piece);
            piece.GetComponent<DeckPiece>().IsDocked = !Docked;
        }
    }
}