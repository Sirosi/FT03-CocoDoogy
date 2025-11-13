using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Tile.Piece.Trigger;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class TriggerCommand: CommandBase
    {
        public override bool IsUserCommand => true;
        
        
        /// <summary>
        /// 트리거 위치
        /// </summary>
        public Vector2Int GridPos = Vector2Int.zero;


        public TriggerCommand(object param): base(CommandType.Trigger, param)
        {
            GridPos = (Vector2Int)param;
        }


        private TriggerPieceBase TriggerPiece
        {
            get
            {
                HexTile tile = HexTile.GetTile(GridPos);
                if (!tile) return null;

                Piece piece = tile.Pieces[(int)HexDirection.Center];
                if (!piece) return null;

                TriggerPieceBase result = piece.GetComponent<TriggerPieceBase>();

                return result;
            }
        }


        public override void Execute()
        {
            TriggerPieceBase trigger = TriggerPiece;
            if (!trigger) return;

            trigger.Interact();
            GimmickExecutor.ExecuteFromTrigger(GridPos, trigger.IsOn);
        }

        public override void Undo()
        {
            TriggerPieceBase trigger = TriggerPiece;
            if (!trigger) return;

            trigger.UnInteract();
            GimmickExecutor.ExecuteFromTrigger(GridPos, trigger.IsOn);
        }
    }
}