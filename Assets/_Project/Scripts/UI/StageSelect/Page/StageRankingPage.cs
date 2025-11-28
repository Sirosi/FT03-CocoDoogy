using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.StageSelect.Item;
using Lean.Pool;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    public class StageRankingPage : StageInfoPage
    {
        [SerializeField] private RankItem prefab;
        [SerializeField] private RectTransform container;
        
        protected override async void OnShowPage()
        {
            foreach (Transform child in container)
            {
                LeanPool.Despawn(child.gameObject);
            }
            var ranking = await FirebaseManager.GetRanking((StageData.theme.ToIndex() + 1).Hex2(), StageData.index.Hex2());
            
            var sortedRanking = ranking.OrderBy(pair => pair.Value.rank).ToList();
            
            foreach (var kvp in sortedRanking)
            {
                RankData rank = kvp.Value;
                
                var rankItem = LeanPool.Spawn(prefab, container);
                rankItem.GetComponent<RankItem>().Init(rank.rank.ToString(),
                    rank.nickname,
                    rank.refillPoints.ToString(),
                    rank.remainAP,
                    rank.clearTime,
                    rank.replayId,
                    StageData);
            }
        }
    }
}