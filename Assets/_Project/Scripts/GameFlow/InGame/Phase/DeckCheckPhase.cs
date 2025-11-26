using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    public class DeckCheckPhase: IPhase
    {
        private Vector2Int? gridPos = null;
        private Vector2Int destination = Vector2Int.zero;
        

        public void OnClear()
        {
            gridPos = null;
            destination = Vector2Int.zero;
        }
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;
            
            // 타일 존재 확인
            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile) return true;

            // 부두 존재 확인
            if (!tile.HasPiece(PieceType.Deck, out Piece piece)) return true;
            
            // 항구 존재 확인 및 
            DeckPiece deck = piece.GetComponent<DeckPiece>();
            if (!deck || !deck.IsDocked) return true;
            if (!piece.Target.HasValue) return true;

            gridPos = PlayerHandler.GridPos;
            destination = piece.Target.Value;
            InGameManager.ChangeInteract(OnTriggerInteracted);

            return false;
        }


        private void OnTriggerInteracted()
        {
            MessageDialog.ShowMessage("승선 확인", "배에 올라탈 거야?", DialogMode.YesNo, OnTriggerControlled);
        }
        private void OnTriggerControlled(CallbackType type)
        {
            gridPos = null;
            if (type == CallbackType.Yes)
            {
                gridPos = destination;
                CommandManager.Sail(destination);
                
                InGameManager.ProcessPhase();
            }
        }
    }
}