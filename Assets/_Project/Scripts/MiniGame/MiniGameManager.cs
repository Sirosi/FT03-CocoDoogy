using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.MiniGame
{
    public class MiniGameManager : Singleton<MiniGameManager>
    {
        [SerializeField] private MiniGameBase[] miniGames;
        [SerializeField] private GameObject backGround;
        public GameObject BackGround => backGround;
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
        public static void OpenRandomGame(System.Action callback)
        {
            Instance.gameObject.SetActive(true);
            Instance.backGround.SetActive(true);
            
            Theme nowTheme = InGameManager.Stage.theme;

            //nowTheme를 갖고 있는 게임을 가져옴//즉, 테마가 없는 게임은 모든 테마를 갖고있도록 설정해야 랜덤게임에 선택될수있음
            MiniGameBase[] possibleGames = Instance.miniGames.Where(x => x.HasWithTheme(nowTheme)).ToArray();
            int randomIdx = Random.Range(0, possibleGames.Length);

            MiniGameBase selectedMiniGame = possibleGames[randomIdx];
            selectedMiniGame.gameObject.SetActive(true);

            selectedMiniGame.Open(callback);
        }
    }
}
