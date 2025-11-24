using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.CameraSwiper.Popup;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    public class DeckCheckPhase : IPhase
    {
        private Vector2Int? gridPos = null;
        private Vector2Int destination = Vector2Int.zero;


        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            // 플레이어 시작 위치에 Trigger같은 걸 두면 안 됨
            // Trigger에 도착한 순간에 ActionPoints가 0이 되면, 그사이에 gridPos가 갱신되는 문제를 해결하기 위함
            if (HexTileMap.StartPos == PlayerHandler.GridPos) return true;
            // 같은 타일에서 무한하게 동작하지 않게 하기 위한 예외처리
            if (gridPos == PlayerHandler.GridPos) return true;
            gridPos = PlayerHandler.GridPos;

            // 타일 존재 확인
            HexTile tile = HexTile.GetTile(gridPos.Value);
            if (!tile) return true;

            // 부두 존재 확인
            if (!tile.HasPiece(PieceType.Deck, out Piece piece)) return true;

            // 항구 존재 확인 및 
            DeckPiece deck = piece.GetComponent<DeckPiece>();
            if (!deck || !deck.IsDocked) return true;
            if (!piece.Target.HasValue) return true;

            gridPos = PlayerHandler.GridPos;
            destination = piece.Target.Value;
            MessageDialog.ShowMessage("승선 확인", "배에 올라탈 거야?", DialogMode.YesNo, OnTriggerControlled);

            return false;
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