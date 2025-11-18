using CocoDoogy.GameFlow.InGame;

namespace CocoDoogy.Tile.Piece.Trigger
{
    public class GravityButtonPiece: TriggerPieceBase
    {
        public override bool IsOn => PlayerHandler.GridPos == Parent.GridPos;
        
        
        public override void OnRelease(Piece data)
        {
            
        }


        public override void Interact()
        {

        }
        public override void UnInteract()
        {
            
        }
    }
}