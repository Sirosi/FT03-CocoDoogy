using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using CocoDoogy.Tile.Piece.Trigger;
using CocoDoogy.UI.Popup;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 작동시킬 수 있는 트리거가 있는지 확인
    /// </summary>
    public class TriggerCheckPhase: IPhase
    {
        private Vector2Int gridPos = Vector2Int.zero;
        
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            // 같은 타일에서 무한하게 동작하지 않게 하기 위한 예외처리
            if (gridPos == PlayerHandler.GridPos) return true;
            gridPos = PlayerHandler.GridPos;

            // 해당 타일에 기믹과 관련된 트리거 존재 확인
            GimmickData[] data = HexTileMap.GetTriggers(gridPos);
            if (data.Length <= 0) return true;

            // 타일 존재 확인
            HexTile tile = HexTile.GetTile(gridPos);
            if (!tile) return true;

            // 중앙 기물 확인
            // Switch, Button이 아니면 Trigger 동작시키는 거 불가능하게
            // GravityButton은 다른 식으로 동작함
            Piece centerPiece = tile.Pieces[(int)HexDirection.Center];
            if (!centerPiece) return true;
            if (centerPiece.BaseData.type is not (PieceType.Switch or PieceType.Button)) return true;

            // TODO: Trigger 동작가능 버튼에 Event를 연결하는 식으로 해야 할 것 같음.
            gridPos = PlayerHandler.GridPos;
            MessageDialog.ShowMessage("기믹 동작", "해당 타일에 있는 래버를 당길거야?", DialogMode.YesNo, OnTriggerControlled);

            return true;
        }

        private void OnTriggerControlled(CallbackType type)
        {
            if (type != CallbackType.Yes) return;
            
            CommandManager.Trigger(gridPos);
        }
    }
}