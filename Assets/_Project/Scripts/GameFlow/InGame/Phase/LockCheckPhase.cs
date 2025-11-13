using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 더 움직일 수 없는 상태인지 체크
    /// </summary>
    public class LockCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (!tile.IsPlaceable || // 타일이 이동불가 상태인지
                tile.CanMovePoses().Count <= 0) // 주변에 이동 가능 타일이 없는지
            {
                CommandManager.Refill();
                return false;
            }
            // TODO: 주변에 이동 가능 타일이 없는지는 아이템 사용 가능 여부도 체크해야 할 수 있음

            
            return true;
        }
    }
}