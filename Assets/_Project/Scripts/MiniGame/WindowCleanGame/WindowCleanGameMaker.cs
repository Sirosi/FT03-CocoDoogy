using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CocoDoogy.Core;

namespace CocoDoogy.MiniGame.WindowCleanGame
{
    
    public class WindowCleanGameMaker : MonoBehaviour
    {

        [SerializeField] GameObject DirtyPrefab;
        [SerializeField] RectTransform dirtyParent;


        [SerializeField] Sprite[] forestDirties;
        [SerializeField] Sprite[] sandDirties;
        [SerializeField] Sprite[] waterDirties;
        [SerializeField] Sprite[] snowDirties;

        private Dictionary<Theme, Sprite> bgDict;
        private Dictionary<Theme, Sprite[]> dirtiesDict;


        private void Awake()
        {
            dirtiesDict = new Dictionary<Theme, Sprite[]>
            {
                {Theme.Forest, forestDirties},
                {Theme.Snow, snowDirties},
                { Theme.Water, waterDirties},
                {Theme.Sand, sandDirties},
            };
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            //테마도 함께 불러야함
            StartWindowCleanGame(Theme.Forest);
            
        }

        /// <summary>
        /// 초기화
        /// </summary>
       

        public void StartWindowCleanGame(Theme thema)
        {

            if(dirtiesDict.TryGetValue(thema, out Sprite[] dirties))
            {
                SummonDirties(dirties);
            }

        }

        /// <summary>
        /// 먼지 생성
        /// </summary>
        /// <param name="dirties"></param>
        void SummonDirties(Sprite[] dirties)
        {
                //위치는 랜덤하게 dirtyParnet의 anchored안에서 최대길이 안에서
                float randomWidth = Random.Range(-dirtyParent.rect.width / 2f, dirtyParent.rect.width/2f);
                float randomHeight = Random.Range(-dirtyParent.rect.height / 2f, dirtyParent.rect.height/2f);
                Vector2 randomPos = new Vector2 (randomWidth,  randomHeight);
                GameObject obj = Instantiate(DirtyPrefab,dirtyParent);
                obj.GetComponent<Image>().sprite = dirties[Random.Range(0, dirties.Length)];
                RectTransform dirtyRect = obj.GetComponent<RectTransform>();
                dirtyRect.localPosition = new Vector2(randomWidth, randomHeight);


        }
    }
}
