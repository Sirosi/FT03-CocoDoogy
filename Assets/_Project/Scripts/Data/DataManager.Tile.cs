using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.Data
{
    public partial class DataManager
    {
        private readonly Dictionary<TileType, HexTileData> tiles = new();
        private readonly Dictionary<PieceType, PieceData> pieces = new();
        private readonly Dictionary<WeatherType, WeatherData> weathers = new();
        
        
        [SerializeField] private HexTileData[] tileData;
        [SerializeField] private PieceData[] pieceData;
        [SerializeField] private WeatherData[] weatherData;


        public static IEnumerable<TileType> TileTypes => Instance?.tiles.Keys;
        public static IEnumerable<PieceType> PieceTypes => Instance?.pieces.Keys;
        public static IEnumerable<WeatherType> WeatherTypes => Instance?.weathers.Keys;


        private void InitTileData()
        {
            foreach (var data in tileData)
            {
                tiles.Add(data.type, data);
            }
            foreach (var data in pieceData)
            {
                pieces.Add(data.type, data);
            }
            foreach (var data in weatherData)
            {
                weathers.Add(data.type, data);
            }
        }
        
        
        public static HexTileData GetTileData(TileType type)
        {
            if (Instance == null) return null;
            
            return Instance.tiles.GetValueOrDefault(type);
        }
        
        public static PieceData GetPieceData(PieceType type)
        {
            if (Instance == null) return null;
            
            return Instance.pieces.GetValueOrDefault(type);
        }

        public static WeatherData GetWeatherData(WeatherType type)
        {
            if (Instance == null) return null;
            
            return Instance.weathers.GetValueOrDefault(type);
        }
    }
}