using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick;
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
    public class TriggerCheckPhase: IPhase, IClearable
    {
        private Vector2Int? gridPos = null;
        

        public void OnClear()
        {
            gridPos = null;
        }
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            InGameManager.ChangeInteract(null);
            // 해당 타일에 기믹과 관련된 트리거 존재 확인
            GimmickData[] data = HexTileMap.GetTriggers(PlayerHandler.GridPos);
            if (data.Length <= 0) return true;

            // 타일 존재 확인
            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile) return true;

            // 중앙 기물 확인
            // Switch, Button이 아니면 Trigger 동작시키는 거 불가능하게
            // GravityButton은 다른 식으로 동작함
            Piece centerPiece = tile.GetPiece(HexDirection.Center);
            PieceType pieceType = centerPiece?.BaseData.type ?? PieceType.None;
            if (pieceType is not (PieceType.Lever or PieceType.Button)) return true;
            
            // 이미 눌린 버튼은 다시 누를 수 없음
            TriggerPieceBase triggerPiece = centerPiece.GetComponent<TriggerPieceBase>();
            if (pieceType is PieceType.Button && triggerPiece.IsOn) return true;

            gridPos = PlayerHandler.GridPos;
            InGameManager.ChangeInteract((pieceType is PieceType.Lever) ? OnLeverInteracted : OnButtonInteracted);

            return false;
        }


        private void OnLeverInteracted()
        {
            MessageDialog.ShowMessage("기믹 동작", "해당 타일에 있는 래버를 당길거야?", DialogMode.YesNo, OnTriggerControlled);
        }
        private void OnButtonInteracted()
        {
            MessageDialog.ShowMessage("기믹 동작", "해당 타일에 있는 버튼을 누를거야?", DialogMode.YesNo, OnTriggerControlled);
        }
        private void OnTriggerControlled(CallbackType type)
        {
            if (type == CallbackType.Yes)
            {
                CommandManager.Trigger(gridPos.Value);
                GimmickExecutor.ExecuteFromTrigger(gridPos.Value);
                InGameManager.ProcessPhase();
            }
        }
    }
}