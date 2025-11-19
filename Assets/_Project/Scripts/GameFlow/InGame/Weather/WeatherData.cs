using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Weather
{
    [CreateAssetMenu(menuName = "Data/Weather Data", fileName = "New Weather Data")]
    public class WeatherData: ScriptableObject
    {
        [Header("Weather Data")]
        public WeatherType type;
        public Sprite icon;
        public string weatherName;
        
        [Header("Editor")]
        [Tooltip("비고")] [TextArea(3, 10)] [SerializeField] private string editorDescription = string.Empty;
    }
}