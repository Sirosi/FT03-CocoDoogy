using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.UI.Popup;
using System;
using System.Threading.Tasks;
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
                // TODO : FirebaseManager.ClearStageAsync 추가
                _ = FirebaseManager.ClearStageAsync(InGameManager.StageData.theme.ToIndex(),
                    InGameManager.StageData.index, InGameManager.ActionPoints, 10.5f);
                
                MessageDialog.ShowMessage("승리", "그래, 이긴 걸로 하자!", DialogMode.Confirm,
                    _ => SceneManager.LoadScene("Lobby"));
                return false;
            }

            return true;
        }
    }
}