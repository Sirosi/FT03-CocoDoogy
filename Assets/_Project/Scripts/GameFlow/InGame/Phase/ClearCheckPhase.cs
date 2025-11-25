using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.Timer;
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
                InGameTimer.ToggleTimer();
                
                int remainAp = InGameManager.RefillPoints * InGameManager.CurrentMapMaxActionPoints + InGameManager.ActionPoints;
                
                _ = FirebaseManager.ClearStageAsync(InGameManager.Stage.theme.ToIndex(),
                    InGameManager.Stage.index, remainAp, InGameTimer.CurrentTime);

                foreach (var itemData in InGameItemUI.UsedItems)
                {
                    if (itemData.Value)
                    {
                        _ = FirebaseManager.UseItemAsync(itemData.Key.itemId);
                    }
                }

                MessageDialog.ShowMessage("승리", "그래, 이긴 걸로 하자!", DialogMode.Confirm,
                    _ => SceneManager.LoadScene("Lobby"));
                
                return false;
            }

            return true;
        }
    }
}