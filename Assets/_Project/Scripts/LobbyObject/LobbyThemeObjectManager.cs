using CocoDoogy.CameraSwiper;
using CocoDoogy.Core;
using CocoDoogy.Network;
using UnityEngine;
using System;

namespace CocoDoogy.LobbyObject
{
    public class LobbyThemeObjectManager : MonoBehaviour
    {
        public GameObject[] forestObjects;
        public GameObject[] waterObjects;
        public GameObject[] snowObjects;
        public GameObject[] sandObjects;

        private Theme lastTheme = Theme.None;
        private int lastClearedStageCount = -1;

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
            var lastStage = await FirebaseManager.GetLastClearStage();
            int cleared = 0;

            if (lastStage != null)
            {
                // theme hex → Theme enum
                Theme theme = (Theme)Convert.ToInt32(lastStage.theme, 16);

                int themeIndex = theme.ToIndex();              // 0~3
                int levelIndex = Convert.ToInt32(lastStage.level, 16); // 1~20

                // 전역 클리어 스테이지 인덱스 계산
                cleared = (themeIndex * 20) + levelIndex;      // 0~79
            }

            lastClearedStageCount = -1;
            TryUpdate(CurrentLobbyTheme(), cleared);
        }

        private Theme CurrentLobbyTheme()
        {
            return PageCameraSwiper.CurrentTheme;
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

            switch (theme)
            {
                case Theme.Forest:
                    themeStartIndex = theme.ToIndex() * forestObjects.Length;
                    ApplyActivation(forestObjects, themeStartIndex, cleared);
                    break;
                case Theme.Water:
                    themeStartIndex = theme.ToIndex() * waterObjects.Length;
                    ApplyActivation(waterObjects, themeStartIndex, cleared);
                    break;
                case Theme.Snow:
                    themeStartIndex = theme.ToIndex() * snowObjects.Length;
                    ApplyActivation(snowObjects, themeStartIndex, cleared);
                    break;
                case Theme.Sand:
                    themeStartIndex = theme.ToIndex() * sandObjects.Length;
                    ApplyActivation(sandObjects, themeStartIndex, cleared);
                    break;
            }
        }

        private void ApplyActivation(GameObject[] objs, int themeStart, int cleared)
        {
            int localCleared = Mathf.Clamp(cleared - themeStart, 0, 20);

            for (int i = 0; i < objs.Length; i++)
                objs[i].SetActive(i < localCleared);
        }
    }
}
