using CocoDoogy.Data;
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
    public class StageSelectButton: MonoBehaviour
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
        public void Init(StageData data, int starSize, Action<StageData> actionCallback)
        {
            if (starSize < 0)
            {
                startButton.GetComponentInChildren<Image>().sprite = lockedSprite;
                starGroup.gameObject.SetActive(false);
                return;
            }
            
            startButton.GetComponentInChildren<Image>().sprite = defaultSprite;
            starGroup.gameObject.SetActive(true);
            
            stageData = data;
            this.callback = actionCallback;

            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }
            
            stageNumberText.text = $"{(int)Mathf.Sqrt((int)data.theme)}-{data.index}";
        }


        private void OnButtonClicked()
        {   
            callback?.Invoke(stageData);
        }
    }
}