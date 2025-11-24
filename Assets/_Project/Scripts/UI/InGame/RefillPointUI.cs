using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Tile;
using UnityEngine;
using TMPro;

namespace CocoDoogy.CameraSwiper.InGame
{
    public class RefillPointUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;


        void OnEnable()
        {
            InGameManager.OnRefillCountChanged += OnRefillCountChanged;
        }
        void OnDisable()
        {
            InGameManager.OnRefillCountChanged -= OnRefillCountChanged;
        }


        private void OnRefillCountChanged(int point)
        {
            text.SetText($"{point} / {HexTileMap.RefillPoint}");
        }
    }
}