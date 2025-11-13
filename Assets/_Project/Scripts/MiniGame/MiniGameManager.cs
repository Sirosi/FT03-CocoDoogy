using CocoDoogy.Core;
using CocoDoogy.MiniGame.CoatArrangeGame;
using CocoDoogy.MiniGame.ToyFindGame;
using CocoDoogy.MiniGame.TrashGame;
using CocoDoogy.MiniGame.UmbrellaGame;
using CocoDoogy.MiniGame.WindowCleanGame;
using CocoDoogy.UI.Popup;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public class MiniGameManager : Singleton<MiniGameManager>
    {
        [SerializeField] private MiniGameBase[] miniGames;

        protected override void Awake()
        {
            base.Awake();
            foreach (var miniGame in miniGames)
            {
                miniGame.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// MiniGameManger가 갖고있는 모든 미니게임중 테마를 받으면서 호출
        /// </summary>
        public void OpenRandomGame()
        {
            gameObject.SetActive(true);
            
            Theme nowTheme = Theme.Forest; // TODO: 나중에 맵 데이터에서 호출하게 변경

            //nowTheme를 갖고 있는 게임을 가져옴//즉, 테마가 없는 게임은 모든 테마를 갖고있도록 설정해야 랜덤게임에 선택될수있음
            MiniGameBase[] possibleGames = miniGames.Where(x => x.HasWithTheme(nowTheme)).ToArray();
            int randomIdx = Random.Range(0, possibleGames.Length);

            MiniGameBase selectedMiniGame = possibleGames[randomIdx];
            selectedMiniGame.gameObject.SetActive(true);

            selectedMiniGame.Open(OnGiveReward);
        }

        /// <summary>
        /// 보상을 얻는함수
        /// </summary>
        public void OnGiveReward()
        {
            gameObject.SetActive(false);
        }
    }
}
