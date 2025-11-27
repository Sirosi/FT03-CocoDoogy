using CocoDoogy.Core;
using CocoDoogy.Data;
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
            stageGroup.position = new Vector2(0, Screen.height * 1.5f);
        }
        
        public void DrawButtons(Theme theme, int start)
        {
            while (spawnedButtons.Count > 0)
            {
                LeanPool.Despawn(spawnedButtons.Pop());
            }
            
            foreach (StageData data in DataManager.GetStageData(theme))
            {
                if (data.index < start) continue; 
                
                StageSelectButton stageButton = LeanPool.Spawn(stageButtonPrefab, stageGroup);
                stageButton.Init(data, 0, StageSelectManager.ShowReadyView);
                
                spawnedButtons.Push(stageButton);
                if (spawnedButtons.Count >= LIST_SIZE) break;
            }
        }
    }
}