using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSelectButton: MonoBehaviour
    {
        [SerializeField] private StageData data;
        
        [SerializeField] private GameObject[] clearStars = null;
        [SerializeField] private CommonButton startButton;


        private StageData stageData = null;


        void Awake()
        {
            if (!data) return;
            Init(data, 3);
        }
        

        public void Init(StageData data, int starSize)
        {
            stageData = data;
            foreach (GameObject star in clearStars)
            {
                star.SetActive(starSize-- > 0);
            }
            
            startButton.onClick.AddListener(OnButtonClicked);
        }


        private void OnButtonClicked()
        {
            InGameManager.MapData = stageData.mapData.text;
            SceneManager.LoadScene("InGame"); // TODO: 임시 기능
        }
    }
}