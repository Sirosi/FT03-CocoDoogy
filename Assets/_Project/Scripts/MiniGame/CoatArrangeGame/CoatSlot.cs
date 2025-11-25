using UnityEngine;

namespace CocoDoogy.MiniGame.CoatArrangeGame
{
    public class CoatSlot : MonoBehaviour
    {
        public int Id;
        private CoatArrangeMiniGame parent;


        public void Init(CoatArrangeMiniGame coatArrangeMiniGame)
        {
            parent = coatArrangeMiniGame;
        }
        /// <summary>
        /// 슬롯과 코트의 ID를 비교하여 클리어여부를 확인하는 함수
        /// </summary>
        public bool CheckID()
        {
            if (transform.childCount == 0) return false;

            Coat coat = transform.GetChild(0).GetComponent<Coat>();
            bool correct = (coat != null && coat.Id == Id);
            return correct;
        }
    }
}
