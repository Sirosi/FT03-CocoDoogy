using CocoDoogy.Data;
using CocoDoogy.Network;
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
        [Header("UI Components")] [SerializeField]
        private TextMeshProUGUI stageNumberText;

        [SerializeField] private CommonButton startButton;

        [SerializeField] private GameObject starGroup = null;
        [SerializeField] private GameObject[] clearStars = null;

        [Header("UI Components")] [SerializeField]
        private Sprite defaultSprite;

        [SerializeField] private Sprite lockedSprite;


        private StageData stageData = null;
        private Action<StageData> callback = null;


#if UNITY_EDITOR
        void Reset()
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

        void Awake()
        {
            startButton.onClick.AddListener(OnButtonClicked);
        }


        /// <summary>
        /// 스테이지 데이터 입력 및 초기화
        /// </summary>
        /// <param name="data">스테이지 데이터</param>
        /// <param name="starSize">스테이지 사이즈</param>
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
        public void Init(StageData data, int starSize, Action<StageData> actionCallback)
        {
            // 먼저 로컬에 저장 (ApplyLockedState에서 사용)
            stageData = data;
            callback = actionCallback;

            // 보이는 UI 초기화(예: 스타)
            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }

            // 기본 텍스트 세팅 (항상 보이게)
            stageNumberText.text = $"{data.stageName}";

            // 안전하게 LastClearedStage 정보 가져오기
            var last = StageSelectManager.LastClearedStage;
            int lastTheme = 0;
            int lastLevel = 0;

            if (last != null)
            {

                lastTheme = last.theme?.Hex2Int() ?? 0;
                lastLevel = last.level?.Hex2Int() ?? 0;
            }

            int dataTheme = (int)data.theme; 
            int dataLevel = data.index; 

            Debug.Log(
                $"Init() called: dataTheme={dataTheme}, dataLevel={dataLevel}, lastTheme={lastTheme}, lastLevel={lastLevel}");

            // 1) 클리어 기록 없음 -> 1-1만 오픈
            if (lastTheme == 0 && lastLevel == 0)
            {
                bool unlocked = (dataTheme == 1 && dataLevel == 1);
                ApplyLockedState(!unlocked);
                Debug.Log($"No record -> unlocked:{unlocked} for {data.stageName}");
                return;
            }

            // 2) 동일 테마일 경우: 마지막 클리어 레벨 + 1 까지 열림
            if (dataTheme == lastTheme)
            {
                bool unlocked = dataLevel <= lastLevel + 1;
                ApplyLockedState(!unlocked);
                Debug.Log($"Same theme -> dataLevel:{dataLevel}, lastLevel:{lastLevel}, unlocked:{unlocked}");
                return;
            }

            // 3) 이전 테마(이미 완전히 클리어한 테마들)는 전부 오픈
            if (dataTheme < lastTheme)
            {
                ApplyLockedState(false);
                Debug.Log($"Older theme -> open {data.stageName}");
                return;
            }

            // 4) 다음 테마(바로 다음 테마) : 1스테이지만 오픈
            if (dataTheme == lastTheme + 1)
            {
                bool unlocked = (dataLevel == 1);
                ApplyLockedState(!unlocked);
                Debug.Log($"Next theme -> unlocked only if level==1 -> unlocked:{unlocked}");
                return;
            }

            // 5) 그 이후 테마: 전부 잠김
            ApplyLockedState(true);
            Debug.Log($"Future theme -> locked {data.stageName}");
        }

        private void ApplyLockedState(bool locked)
        {
            // stageData는 이미 Init 초반에 할당되어 있어야 함
            startButton.interactable = !locked;
            var img = startButton.GetComponentInChildren<Image>();
            if (img != null)
                img.sprite = locked ? lockedSprite : defaultSprite;

            if (starGroup != null)
                starGroup.gameObject.SetActive(!locked);

            if (stageNumberText != null && stageData != null)
                stageNumberText.text = $"{stageData.stageName}";
        }

        private void OnButtonClicked()
        {
            callback?.Invoke(stageData);
        }
    }
}