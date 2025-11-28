using CocoDoogy.Network;
using CocoDoogy.UI.StageSelect.Item;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    public class StageRankingPage : StageInfoPage
    {
        [SerializeField] private RankItem prefab;
        [SerializeField] private RectTransform container;

        private void OnEnable()
        {
            _ = FirebaseManager.GetRanking((StageData.theme.ToIndex() + 1).Hex2(), StageData.index.Hex2());
        }
        protected override void OnShowPage()
        {
            // _ = FirebaseManager.GetRanking("01", "01");
            // TODO : 페이지가 열렸을 때 해당 스테이지의 1등부터 10등 까지의 랭킹 정보를 띄워야 함.
            // RankItem rankItem = Instantiate(prefab, container);
            // rankItem.Init("#1","닉네임","0/1","1","1.5");
        }
    }
}