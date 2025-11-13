using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSelectButton: MonoBehaviour
    {
        [SerializeField] private GameObject[] clearStars = null;


        void Awake()
        {
            // TODO: Init에 인자를 넣는 시점에 제거
            Init();
        }
        

        public void Init() // TODO: 나중에 초기화용 인자 필요
        {
            int starSize = 3; // TODO: 나중에 인자로 가야 함
            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }
        }
    }
}