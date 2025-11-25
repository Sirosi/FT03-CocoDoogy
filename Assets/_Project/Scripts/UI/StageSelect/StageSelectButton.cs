using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.Network;
using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect
{
    /// <summary>
    /// Stage 선택 버튼
    /// </summary>
    public class StageSelectButton : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI stageNumberText;

        [SerializeField] private CommonButton startButton;

        [SerializeField] private GameObject starGroup = null;
        [SerializeField] private GameObject[] clearStars = null;

        [Header("UI Components")] 
        [SerializeField] private Sprite defaultSprite;

        [SerializeField] private Sprite lockedSprite;


        private StageData stageData = null;
        private Action<StageData> callback = null;

#if UNITY_EDITOR
        private void Reset()
        {
            stageNumberText = GetComponentInChildren<TextMeshProUGUI>();
            startButton = GetComponentInChildren<CommonButton>();

            List<GameObject> starObjects = new();
            foreach (Transform child in transform.Find("Stars"))
            {
                starObjects.Add(child.gameObject);
            }

            clearStars = starObjects.ToArray();
        }
#endif

        private void Awake()
        {
            startButton.onClick.AddListener(OnButtonClicked);
        }


        /// <summary>
        /// 스테이지 데이터 입력 및 초기화
        /// </summary>
        /// <param name="data">스테이지 데이터</param>
        /// <param name="starSize">스테이지 사이즈</param>
        /// <param name="actionCallback"></param>
        public void Init(StageData data, int starSize, Action<StageData> actionCallback)
        {
            stageData = data;
            callback = actionCallback;

            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }

            stageNumberText.text = $"{data.stageName}";

            var last = StageSelectManager.LastClearedStage;
            int lastTheme = 0;
            int lastLevel = 0;

            if (last != null)
            {
                lastTheme = last.theme?.Hex2Int() ?? 0;
                lastLevel = last.level?.Hex2Int() ?? 0;
            }

            int dataTheme = data.theme.ToIndex() + 1;
            int dataLevel = data.index;
            
            bool unlocked = true;
            foreach (var check in IsStageOpen)
            {
                if (!check(dataTheme, dataLevel, lastTheme, lastLevel))
                {
                    unlocked = false;
                    break;
                }
            }
            
            ApplyLockedState(unlocked);
        }
        
        // TODO: 스테이지 테스트 할때 위에 Init 비활성화 하고 이거 활성화 해서 사용하면 됨 
        
        // public void Init(StageData data, int starSize, Action<StageData> actionCallback)
        // {
        //     if (StageSelectManager.LastClearedStage.theme.Hex2Int() < (int)data.theme ||
        //         StageSelectManager.LastClearedStage.level.Hex2Int() < data.index - 1)
        //     {
        //         startButton.interactable = false;
        //         startButton.GetComponentInChildren<Image>().sprite = lockedSprite;
        //         starGroup.gameObject.SetActive(false);
        //         stageNumberText.text = $"{data.stageName}";
        //         return;
        //     }
        //
        //     startButton.interactable = true;
        //     startButton.GetComponentInChildren<Image>().sprite = defaultSprite;
        //     starGroup.gameObject.SetActive(true);
        //
        //     stageData = data;
        //     callback = actionCallback;
        //
        //     foreach (GameObject star in clearStars)
        //     {
        //         star.SetActive(starSize-- > 0);
        //     }
        //
        //     stageNumberText.text = $"{data.stageName}";
        // }
        
        private void ApplyLockedState(bool locked)
        {
            startButton.interactable = !locked;
            var img = startButton.GetComponentInChildren<Image>();
            if (img)
                img.sprite = locked ? lockedSprite : defaultSprite;

            if (starGroup)
                starGroup.gameObject.SetActive(!locked);

            if (stageNumberText && stageData)
                stageNumberText.text = $"{stageData.stageName}";
        }

        private void OnButtonClicked()
        {
            callback?.Invoke(stageData);
        }

        #region < 스테이지 활성화 책임 연쇄 패턴>
        private static Func<int, int, int, int, bool>[] IsStageOpen =
        {
            HasRecord, IsSameTheme, IsPrevTheme, IsNextTheme
        };
	
        /// <summary>
        /// 클리어 기록이 있는가? <br/>
        /// 없으면 1-1 스테이지만 오픈
        /// </summary>
        /// <param name="dataTheme">선택한 테마</param>
        /// <param name="dataLevel">선택한 테마에 있는 스테이지</param>
        /// <param name="lastTheme">가장 마지막으로 클리어한 테마</param>
        /// <param name="lastLevel">가장 마지막으로 클리어한 스테이지</param>
        /// <returns></returns>
        private static bool HasRecord(int dataTheme, int dataLevel, int lastTheme, int lastLevel)
        {
            if (lastTheme == 0 && lastLevel == 0)
                return !(dataTheme == 1 && dataLevel == 1);
            return true;
        }

        /// <summary>
        /// 클리어 기록이 있고 선택한 테마가 마지막 클리어한 테마와 같은 테마인가? <br/>
        /// 마지막 클리어에서 +1 한 스테이지 오픈
        /// </summary>
        private static bool IsSameTheme(int dataTheme, int dataLevel, int lastTheme, int lastLevel)
        {
            if (dataTheme == lastTheme)
                return !(dataLevel <= lastLevel + 1);
            return true;
        }

        /// <summary>
        /// 클리어한 기록이 있고 이미 완전히 클리어한 테마인가? <br/>
        /// 1테마가 올 클리어 상태이면 1테마 전부 오픈
        /// </summary>
        private static bool IsPrevTheme(int dataTheme, int dataLevel, int lastTheme, int lastLevel)
        {
            return !(dataTheme < lastTheme);
        }

        /// <summary>
        /// 클리어한 기록이 있고 이미 완전히 클리어한 테마이면 다음 테마의 1스테이지 오픈
        /// </summary>
        private static bool IsNextTheme(int dataTheme, int dataLevel, int lastTheme, int lastLevel)
        {
            if (StageSelectManager.LastClearedStage == null || dataTheme != lastTheme + 1) return true;
            
            var list = DataManager.GetStageData((Theme)(1 << StageSelectManager.LastClearedStage.theme.Hex2Int()));
            if (list == null) return false;
            int lastStageInPrevTheme = list.Count;
            bool prevThemeCleared = lastLevel >= lastStageInPrevTheme;
            return !(prevThemeCleared && dataLevel == 1);
        }
        #endregion
    }
}