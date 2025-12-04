using CocoDoogy.CameraSwiper;
using CocoDoogy.Core;
using CocoDoogy.Network;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CocoDoogy.LobbyObject
{
    public class LobbyThemeObjectManager : MonoBehaviour
    {
        private Dictionary<Theme, int> stageNum;
        
        public GameObject[] forestObjects;
        public GameObject[] waterObjects;
        public GameObject[] snowObjects;
        public GameObject[] sandObjects;

        private Theme lastTheme = Theme.None;
        private int lastClearedStageCount = -1;

        private void Awake()
        {
            stageNum = new()
            {
                { Theme.Forest, forestObjects.Length },
                { Theme.Water, waterObjects.Length },
                { Theme.Snow, snowObjects.Length },
                { Theme.Sand, sandObjects.Length },
            };
        }
        
        private void OnEnable()
        {
            PageCameraSwiper.OnEndPageChanged += OnThemeChanged;
            ForceRefresh(); // Firebase에서 최신 데이터 로드
        }

        private void OnDisable()
        {
            PageCameraSwiper.OnEndPageChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(Theme theme)
        {
            TryUpdate(theme, lastClearedStageCount);
        }

        private async void ForceRefresh()
        {
            var lastStage = await FirebaseManager.GetLastClearStage(FirebaseManager.Instance.Auth.CurrentUser.UserId);
            int cleared = 0;

            if (lastStage != null)
            {
                // theme hex → Theme enum
                Theme theme = (Theme)Convert.ToInt32(lastStage.theme, 16);

                int themeIndex = theme.ToIndex();             
                int levelIndex = lastStage.level.Hex2Int(); 

                cleared = (themeIndex * stageNum[theme]) + levelIndex;    
            }

            lastClearedStageCount = -1;
            TryUpdate(PageCameraSwiper.CurrentTheme, cleared);
        }

        private void TryUpdate(Theme theme, int cleared)
        {
            if (theme == lastTheme && cleared == lastClearedStageCount)
                return;

            UpdateThemeObjects(theme, cleared);

            lastTheme = theme;
            lastClearedStageCount = cleared;
        }

        private void UpdateThemeObjects(Theme theme, int cleared)
        {
            int themeStartIndex;

            if (theme == Theme.Forest)
            {
                themeStartIndex = theme.ToIndex() * forestObjects.Length;
                ApplyActivation(forestObjects, themeStartIndex, cleared, theme);
            }
            if (theme == Theme.Water)
            {
                themeStartIndex = theme.ToIndex() * waterObjects.Length;
                ApplyActivation(waterObjects, themeStartIndex, cleared, theme);
            }
            if (theme == Theme.Snow)
            {
                themeStartIndex = theme.ToIndex() * snowObjects.Length;
                ApplyActivation(snowObjects, themeStartIndex, cleared, theme);
            }
            if (theme == Theme.Sand)
            {
                themeStartIndex = theme.ToIndex() * sandObjects.Length;
                ApplyActivation(sandObjects, themeStartIndex, cleared, theme);
            }
        }

        private void ApplyActivation(GameObject[] objs, int themeStart, int cleared, Theme theme)
        {
            int max = stageNum[theme];
            int localCleared = Mathf.Clamp(cleared - themeStart, 0, max);
            Debug.Log($"Theme:{theme},cleared:{cleared},themeStart:{themeStart}, max:{max},localCleared:{localCleared}");
            for (int i = 0; i < objs.Length; i++)
                objs[i].SetActive(i < localCleared);
        }
    }
}
