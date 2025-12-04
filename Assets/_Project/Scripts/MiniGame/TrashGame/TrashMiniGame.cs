using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CocoDoogy.MiniGame.TrashGame
{
    public class TrashMiniGame : MiniGameBase
    {
        [SerializeField] private int trashCount = 5;
        [SerializeField] private Trash trashPrefab;
        [SerializeField] private TrashCan trashCanPrefab;
        [SerializeField] private RectTransform trashGamePanel;
        [SerializeField] private RectTransform trashParent;

        [Tooltip("테마별 쓰레기")]
        [SerializeField] private Sprite[] forestTrashes;
        [SerializeField] private Sprite[] sandTrashes;
        [SerializeField] private Sprite[] waterTrashes;
        [SerializeField] private Sprite[] snowTrashes;

        [Tooltip("테마별 쓰레기통")]
        [SerializeField] private Sprite forestTrashCan;
        [SerializeField] private Sprite sandTrashCan;
        [SerializeField] private Sprite waterTrashCan;
        [SerializeField] private Sprite snowTrashCan;

        [Tooltip("테마별 배경")]
        [SerializeField] private Sprite forestBackgroudSprite;
        [SerializeField] private Sprite sandBackgroundSprite;
        [SerializeField] private Sprite waterBackgroundSprite;
        [SerializeField] private Sprite snowBackgroundSprite;

        private Dictionary<Theme, Sprite[]> trashDict;
        private Dictionary<Theme, Sprite> trashcanDict;
        private Dictionary<Theme, Sprite> backgroundDict;

        private readonly List<Trash> trashes = new();


        private void Awake()
        {
            trashDict = new Dictionary<Theme, Sprite[]>
            {
                {Theme.Forest, forestTrashes },
                {Theme.Sand, sandTrashes },
                {Theme.Water, waterTrashes },
                {Theme.Snow, snowTrashes },
            };

            trashcanDict = new Dictionary<Theme, Sprite> {
                {Theme.Forest, forestTrashCan},
                {Theme.Sand, sandTrashCan},
                {Theme.Water, waterTrashCan},
                {Theme.Snow, snowTrashCan},
            };

            backgroundDict = new Dictionary<Theme, Sprite> {
                {Theme.Forest, forestBackgroudSprite},
                {Theme.Sand, sandBackgroundSprite },
                {Theme.Water, waterBackgroundSprite },
                {Theme.Snow, snowBackgroundSprite },
            };
        }


        protected override void ShowRemainCount()
        {
            remainCount.gameObject.SetActive(true);
            remainCount.text = trashes.Count.ToString()+$"/{trashCount}";
        }

        protected override bool IsClear() => trashes.Count <= 0;

        /// <summary>
        /// 게임시작시
        /// </summary>
        protected override void OnOpenInit()
        {
            Theme nowTheme = InGameManager.Stage.theme;  //TODO: 나중에 맵 데이터에서 호출하게 변경
            trashes.Clear();
            StartTrashGame(nowTheme);
        }

        protected override void Disable()
        {
            remainCount.gameObject.SetActive(false);
            foreach (Transform child in trashParent)
            {
                Destroy(child.gameObject);
            }
        }


        /// <summary>
        /// 테마별로 실행
        /// </summary>
        /// <param name="theme"></param>
        public void StartTrashGame(Theme theme)
        {
            if(backgroundDict.TryGetValue(theme, out Sprite background))
            {
                SetBackground(background);
            }
            ResizePanel();
            if (trashDict.TryGetValue(theme, out Sprite[] trashes))
            {
                SummonTrash(trashes);
            }

            if (trashcanDict.TryGetValue(theme, out Sprite trashcan))
            {
                SummonTrashCan(trashcan);
            }
        }
        protected override void SetBackground(Sprite sprite)
        {
            background.sprite = sprite;
        }

        /// <summary>
        /// UI 패널(windowGamePanel)의 크기를 배경 이미지(background.sprite)의 비율에 맞게 조정하는 함수
        /// 왜 필요하냐. Unity에서 Image 컴포넌트의 Preserve Aspect 옵션을 켜면, 이미지가 원본 비율대로 맞춰지는데
        /// 부모 RectTransform 크기와 다를 수 있기 때문에, 실제 패널 크기도 비율에 맞게 조정해야 화면이 어긋나지 않는다.
        /// </summary>
        void ResizePanel()
        {
            if (background == null || background.sprite == null || trashGamePanel == null)
                return;

            RectTransform panelRect = trashGamePanel.GetComponent<RectTransform>();
            RectTransform bgRect = background.rectTransform;
            Sprite sprite = background.sprite;

            // 현재 Image가 PreserveAspect로 표시될 때 실제 화면에 표시되는 크기 계산
            float spriteRatio = sprite.rect.width / sprite.rect.height;
            float rectRatio = bgRect.rect.width / bgRect.rect.height;

            float width, height;
            //배경패널이 사용 이미지보다 너비가 더 넓으면 세로기준 맞춤
            if (rectRatio > spriteRatio)
            {
                // 세로 기준 맞춤
                height = bgRect.rect.height;
                width = height * spriteRatio;
            }
            //배경패널이 사용 이미지보다 높이가 크면 가로기준 맞춤
            else
            {
                // 가로 기준 맞춤
                width = bgRect.rect.width;
                height = width / spriteRatio;
            }

            //계산한 width, height를 스트레치된 패널에 적용
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        
        
        /// <summary>
        /// 쓰레기소환
        /// </summary>
        /// <param name="trashes"></param>
        void SummonTrash(Sprite[] trashes)
        {
         
            float panelWidth = trashParent.rect.width;
            float panelHeight = trashParent.rect.height;

            float halfWidth = panelWidth * 0.5f;

            // Y축 위치선정 아래에서 위로 정도 위치(숫자를 키울수록 아래로감)
            float fixedY = -panelHeight * 0.33f;

            for (int i = 0; i < trashCount; i++)
            {
                Sprite sprite = trashes[Random.Range(0, trashes.Length)];
                float randomX = Random.Range(-halfWidth+50, halfWidth-50);

                Trash trash = Instantiate(trashPrefab, trashParent);
                RectTransform rt = trash.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(randomX, fixedY);

                //패널 비율 기반으로 크기 조정
                float trashWidth = panelWidth * 0.07f;

                float spriteRatio = sprite.rect.height / sprite.rect.width;
                float trashHeight = trashWidth * spriteRatio;
                rt.sizeDelta = new Vector2(trashWidth, trashHeight);

                trash.Init(this, sprite);
                this.trashes.Add(trash);
            }
        }

        /// <summary>
        /// 쓰레기통 소환
        /// </summary>
        /// <param name="trashcanSprite"></param>
        void SummonTrashCan(Sprite trashcanSprite)
        {
         
            float panelWidth = trashParent.rect.width;
            float panelHeight = trashParent.rect.height;

            float halfWidth = panelWidth * 0.5f;
            float randomX = Random.Range(-halfWidth+50, halfWidth-50);

            // Y축 위치선정 아래에서 위로 정도 위치(숫자를 키울수록 아래로감)
            float fixedY = -panelHeight * 0.43f;

            TrashCan trashCan = Instantiate(trashCanPrefab, trashParent);
            RectTransform rt = trashCan.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(randomX, fixedY);

            //패널 크기 비율로 TrashCan 크기 조정
            float canWidth = panelWidth * 0.1f; 

            float ratio = trashcanSprite.rect.height / trashcanSprite.rect.width;
            float canHeight = canWidth * ratio;
            rt.sizeDelta = new Vector2(canWidth, canHeight);

            Image img = trashCan.GetComponent<Image>();
            img.sprite = trashcanSprite;

        }


        /// <summary>
        /// 쓰레기통에 넣을시 클리어판정을 체크하고 Trash는 제거
        /// </summary>
        /// <param name="trash"></param>
        public void DestroyTrash(Trash trash)
        {
            trashes.Remove(trash);
            Destroy(trash.gameObject);
            CheckClear();
        }
    }
}
