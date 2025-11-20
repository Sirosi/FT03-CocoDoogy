using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Test
{
    public class StageSelectTest: MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonGroup;
        [SerializeField] private TextAsset[] mapData;


        void Awake()
        {
            foreach (TextAsset data in mapData)
            {
                string mapName = data.name;
                mapName = mapName.Replace("stage", "").Replace("_", "-");
                
                GameObject button = Instantiate(buttonPrefab, buttonGroup);
                CommonButton commonButton = button.GetComponent<CommonButton>();
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = mapName;
                commonButton.onClick.AddListener(() =>
                {
                    string json = data.text;
                    //InGameManager.MapData = json;
                    SceneManager.LoadScene("InGame");
                });
            }
            
            Destroy(buttonPrefab);
        }
    }
}