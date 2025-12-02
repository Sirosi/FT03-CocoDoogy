using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Friend;
using CocoDoogy.UI.StageSelect.Item;
using Lean.Pool;
using System;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect.Page
{
    public class StageRankingPage : StageInfoPage
    {
        public int CurrentStageStar { get; set; } = 0;
        [SerializeField] private RankItem prefab;
        [SerializeField] private RectTransform container;

        private void OnDisable()
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                LeanPool.Despawn(container.GetChild(i).gameObject);
            }
        }

        protected override async void OnShowPage()
        {
            var ranking = await FirebaseManager.GetRanking((StageData.theme.ToIndex() + 1).Hex2(), StageData.index.Hex2());
            
            var sortedRanking = ranking.OrderBy(pair => pair.Value.rank).ToList();
            Debug.Log($"GetStageClearResult : sortedRanking = {sortedRanking.Count}");
            foreach (var kvp in sortedRanking)
            {
                RankData rank = kvp.Value;
                
                RankItem rankItem = LeanPool.Spawn(prefab, container);
                rankItem.Init(rank.rank.ToString(),
                    rank.nickname,
                    rank.refillPoints.ToString(),
                    rank.remainAP,
                    rank.clearTime,
                    rank.replayId,
                    CurrentStageStar,
                    StageData);
            }
        }
    }
}