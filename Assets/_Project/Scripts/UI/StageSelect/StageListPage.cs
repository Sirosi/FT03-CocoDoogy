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
                // TODO : 네트워크 싱크 이슈. 버튼이 생성되는 중에 나갔다가 들어오면 겹쳐서 생성되는 이슈가 있음.
                int star = await FirebaseManager.GetStar(data.theme.ToIndex() + 1, data.index);
                StageSelectButton stageButton = LeanPool.Spawn(stageButtonPrefab, stageGroup);
                stageButton.Init(data, star, StageSelectManager.ShowReadyView);
                
                spawnedButtons.Push(stageButton);
                if (spawnedButtons.Count >= LIST_SIZE) break;
            }
        }
    }
}