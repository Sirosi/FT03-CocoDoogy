using CocoDoogy.Network;
using System;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSelectStar : MonoBehaviour
    {
        public static int StarCount = 0;
        [SerializeField] private GameObject[] stageStar;

        private void Awake()
        {
            Init();
        }
        
        private void OnDisable()
        {
            Init();
        }
        private void Init()
        {
            foreach (GameObject star in stageStar)
            {
                star.SetActive(false);
            }
        }
        
        public void BrightStar(int star = 1)
        {
            for (int i = 0; i < star; i++)
            {
                stageStar[i].SetActive(true);
            }
        }
    }
}