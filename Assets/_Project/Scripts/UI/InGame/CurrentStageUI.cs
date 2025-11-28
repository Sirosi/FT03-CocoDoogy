using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.UI.StageSelect;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    public class CurrentStageUI : MonoBehaviour
    {
        private TextMeshProUGUI currentStageText;

        private Dictionary<Theme, string> convertKr = new()
        {
            { Theme.Forest, "숲" }, { Theme.Water, "물" }, { Theme.Snow, "눈" }, { Theme.Sand, "사막" }
        }; 
        
        private void Awake()
        {
            currentStageText = GetComponentInChildren<TextMeshProUGUI>();
            currentStageText.text = $"{convertKr[InGameManager.Stage.theme]} 테마 - {InGameManager.Stage.index}";
        }
    }
}
