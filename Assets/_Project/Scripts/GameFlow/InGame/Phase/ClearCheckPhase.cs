using CocoDoogy.Tile;
using CocoDoogy.CameraSwiper.Popup;
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
                MessageDialog.ShowMessage("승리", "그래, 이긴 걸로 하자!", DialogMode.Confirm, _ => SceneManager.LoadScene("Lobby"));
                return false;
            }

            return true;
        }
    }
}