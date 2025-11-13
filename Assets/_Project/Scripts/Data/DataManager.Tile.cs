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
        
        
        [SerializeField] private HexTileData[] tileData;
        [SerializeField] private PieceData[] pieceData;


        public static IEnumerable<TileType> TileTypes => Instance?.tiles.Keys;
        public static IEnumerable<PieceType> PieceTypes => Instance?.pieces.Keys;


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
    }
}