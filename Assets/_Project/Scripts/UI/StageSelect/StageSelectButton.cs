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
            // TODO : 클리어 기록에 따라서 버튼의 활성화 여부 정해지게 변경해야 함.
            // DB에는 클리어한 스테이지의 정보만 가지고 있어서 내림차순으로 돌려서 가장 위의 정보가 마지막 스테이지 정보
            // 여기에서 level, theme를 가져와서 data의 index와 theme를 비교해서 클리어 여부 확인 가능.
            // 생각해보니까 다음 스테이지를 클리어하기 전까지 한번 정보를 찾은다음 계속 가지고 있어야 되는거 아닌가?
            // 그렇지 않으면 스테이지 선택 창을 왔다갔다 하는거로 엄청나게 많은 손실이 발생할거 같은데?
            // 흠... -> 
            Debug.Log(StageSelectManager.LastClearedStage);
            if (starSize < 0)
            {
                startButton.interactable = false;
                startButton.GetComponentInChildren<Image>().sprite = lockedSprite;
                starGroup.gameObject.SetActive(false);
                return;
            }
            
            startButton.interactable = true;
            startButton.GetComponentInChildren<Image>().sprite = defaultSprite;
            starGroup.gameObject.SetActive(true);
            
            stageData = data;
            callback = actionCallback;

            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }
            
            stageNumberText.text = $"{data.stageName}";
        }


        private void OnButtonClicked()
        {   
            callback?.Invoke(stageData);
        }
    }
}