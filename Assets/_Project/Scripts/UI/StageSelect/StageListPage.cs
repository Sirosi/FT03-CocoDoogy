using CocoDoogy.Core;
using CocoDoogy.Data;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.CameraSwiper.StageSelect
{
    public class StageListPage : MonoBehaviour
    {
        public const int LIST_SIZE = 100;


        [SerializeField] private StageSelectButton stageButtonPrefab;
        [SerializeField] private Transform stageGroup;


        private Stack<StageSelectButton> spawnedButtons = new();


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
                stageButton.Init(data, 3, StageSelectManager.ShowReadyView);

                spawnedButtons.Push(stageButton);
                if (spawnedButtons.Count >= LIST_SIZE) break;
            }
        }
    }
}