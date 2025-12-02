using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageListPage: MonoBehaviour
    {
        public const int LIST_SIZE = 100;
        [SerializeField] private StageSelectButton stageButtonPrefab;
        [SerializeField] private Transform stageGroup;


        private Stack<StageSelectButton> spawnedButtons = new();


        void OnEnable()
        {
            stageGroup.position = new Vector2(0, stageGroup.position.y);
        }
        
        public async void DrawButtons(Theme theme, int start)
        {
            while (spawnedButtons.Count > 0)
            {
                LeanPool.Despawn(spawnedButtons.Pop());
            }
            
            foreach (StageData data in DataManager.GetStageData(theme))
            {
                if (data.index < start) continue;
                int star = await FirebaseManager.GetStar(data.theme.ToIndex() + 1, data.index);
                StageSelectButton stageButton = LeanPool.Spawn(stageButtonPrefab, stageGroup);
                stageButton.Init(data, star, StageSelectManager.ShowReadyView);
                
                spawnedButtons.Push(stageButton);
                if (spawnedButtons.Count >= LIST_SIZE) break;
            }
        }
    }
}