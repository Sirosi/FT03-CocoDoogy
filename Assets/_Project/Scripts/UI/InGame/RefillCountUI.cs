using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Tile;
using UnityEngine;
using TMPro;

namespace CocoDoogy.UI.InGame
{
    public class RefillCountUI: MonoBehaviour
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
            text.SetText($"{(HexTileMap.RefillCount + 1 - point)} / {HexTileMap.RefillCount}");
        }
    }
}