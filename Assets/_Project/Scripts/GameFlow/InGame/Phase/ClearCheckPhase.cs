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
                string saveJson = CommandManager.Save();
                
                _ = FirebaseManager.ClearStageAsync(InGameManager.Stage.theme.ToIndex() + 1,
                    InGameManager.Stage.index, remainAp, time, saveJson);
                
                ItemHandler.UseItem();
                
                GameEndPopup.OpenPopup(false);
                
                
                
                InGameManager.Timer.Stop();
                
                // MessageDialog.ShowMessage("승리", "그래, 이긴 걸로 하자!", DialogMode.Confirm, _ => SceneManager.LoadScene("Lobby"));
                
                return false;
            }

            return true;
        }
    }
}