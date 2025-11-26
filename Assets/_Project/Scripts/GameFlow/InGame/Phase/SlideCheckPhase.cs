using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 현재 타일이 슬라이딩 가능 타일인지 체크
    /// </summary>
    public class SlideCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            HexTile tile = HexTile.GetTile(PlayerHandler.GridPos);
            if (tile.CurrentData.moveType != MoveType.Slide) return true; // 현재 타일이 미끄러지는 타일이 아니면 정지
            if (!tile.CanMove(PlayerHandler.LookDirection)) return true; // 바라보는 방향으로 갈 수 없으면 정지

            InGameManager.RegenActionPoint(1, false);
            HexTile nextTile = tile;
            
            while(true)
            {
                if (!nextTile.CanMove(PlayerHandler.LookDirection)) break;

                nextTile = HexTile.GetTile(nextTile.GridPos.GetDirectionPos(PlayerHandler.LookDirection));

                if(nextTile.CurrentData.moveType != MoveType.Slide) break;
            }
            
            InGameManager.ConsumeActionPoint(1, false);
            
            CommandManager.Slide(nextTile.GridPos);
            
            return false;
        }
    }
}