using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using UnityEngine.SceneManagement;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 클리어 여부 확인
    /// </summary>
    public class ClearCheckPhase : IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            if (PlayerHandler.GridPos == HexTileMap.EndPos)
            {
                float time = InGameManager.Timer.NowTime;
                int remainAp = InGameManager.RefillPoints * InGameManager.CurrentMapMaxActionPoints + InGameManager.ActionPoints;
                int refillPoints = InGameManager.RefillPoints;
                string saveJson = CommandManager.Save();
                
                // 여기서 Timer.Stop을 하면 Popup에 0초로 기록됨. 그래서 일단 시간을 멈추고
                InGameManager.Timer.Pause();
                
                ItemHandler.UseItem();
                
                // 이 부분에서 popup이 열리고나서 시간이 초기화 되게 
                _ = FirebaseManager.ClearStageAsync(InGameManager.Stage.theme.ToIndex() + 1,
                    InGameManager.Stage.index, remainAp, refillPoints, time, saveJson, 
                    () =>
                    {
                        GameEndPopup.OpenPopup(false);
                        InGameManager.Timer.Stop();
                    });
                
                return false;
            }
            return true;
        }
    }
}