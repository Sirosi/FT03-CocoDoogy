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
            if (InGameManager.ActionPoints < nextTile.CurrentData.RealMoveCost)
            {
                if (InGameManager.RefillPoints <= 1)
                {
                    ProcessDefeat();
                    return false;
                }
                //CommandManager.Refill();
            }

            if (InGameManager.RefillPoints > 0) return true;
            
            // TODO: 상징적인 패배를 넣어야 함.
            ProcessDefeat();
            return false;
        }

        private void ProcessDefeat()
        {
            // TODO: 추후, 아이템을 사용할 거냔 그런 거 넣어야 함
            MessageDialog.ShowMessage("미아", "집을 영구적으로 잃었습니다.", DialogMode.Confirm, _ => SceneManager.LoadScene("Lobby"));
        }
    }
}