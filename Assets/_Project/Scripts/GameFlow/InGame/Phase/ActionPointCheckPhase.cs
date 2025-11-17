using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using UnityEngine.SceneManagement;

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
            if (InGameManager.ActionPoints < nextTile.CurrentData.moveCost)
            {
                if (InGameManager.RefillPoints <= 1)
                {
                    // TODO: 상징적인 패배를 넣어야 함.
                    MessageDialog.ShowMessage("미아", "집을 영구적으로 잃었습니다.", DialogMode.Confirm, _ => SceneManager.LoadScene("Lobby"));
                    return false;
                }
                CommandManager.Refill();
                return false;
            }

            return true;
        }
    }
}