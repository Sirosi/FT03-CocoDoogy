using CocoDoogy.UI.StageSelect;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    public class CurrentStageUI : MonoBehaviour
    {
        private TextMeshProUGUI currentStageText;

        private void Awake()
        {
            currentStageText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            currentStageText.text = $"{StageSelectManager.LastClearedStage.theme} 테마 - {StageSelectManager.LastClearedStage.level}";
        }
    }
}
