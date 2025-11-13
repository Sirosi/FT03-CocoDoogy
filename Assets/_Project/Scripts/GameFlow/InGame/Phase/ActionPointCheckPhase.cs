using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// ActionPoint가 부족해서 이동 불가에 빠졌는지 체크
    /// </summary>
    public class ActionPointCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;
            
            HexTile nextTile = HexTile.GetTile(PlayerHandler.GridPos);
            if (InGameManager.ActionPoint < nextTile.CurrentData.moveCost)
            {
                CommandManager.Refill();
                return false;
            }

            return true;
        }
    }
}