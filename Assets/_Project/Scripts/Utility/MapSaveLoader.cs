using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.Utility
{
    public static class MapSaveLoader
    {
        /// <summary>
        /// 맵 Save 완료 event
        /// </summary>
        public static event Action OnMapSaved = null;
        /// <summary>
        /// 맵 Load 완료 event
        /// </summary>
        public static event Action OnMapLoaded = null;


        /// <summary>
        /// HexTileMap을 JSON규격의 string으로 변경
        /// </summary>
        /// <returns>저장한 JSON</returns>
        public static string ToJson()
        {
            // Map 정보 저장
            Map mapData = new()
            {
                RefillCount = HexTileMap.RefillCount,
                ActionPoint = HexTileMap.ActionPoint,
                DefaultWeather = HexTileMap.DefaultWeather,
                StartPos = HexTileMap.StartPos,
                EndPos = HexTileMap.EndPos,
            };

            List<MapTile> tiles = new(); // tile 정보를 저장할 List
            foreach (HexTile tile in HexTile.Tiles.Values)
            {
                // Tile에 대한 기본 정보
                MapTile tileData = new()
                {
                    Type = tile.BaseData.type,
                    GridPos = tile.GridPos
                };

                // 기물 정보 저장
                for (int i = 0; i < tileData.Pieces.Length; i++)
                {
                    if (tile.Pieces[i] is null) continue;
                    tileData.Pieces[i].Type = tile.Pieces[i].BaseData.type;
                    tileData.Pieces[i].Data = tile.Pieces[i].SpecialData;
                    
                    if (tile.Pieces[i].Target == null) continue;
                    mapData.PieceToTargets.Add(new()
                    {
                        PiecePos = tile.GridPos,
                        TargetPos = (Vector2Int)tile.Pieces[i].Target
                    });
                }

                // 중앙 기물은 LookDirection을 가져야 함으로 Center일 경우 추가 정보 저장
                Piece centerPiece = tile.GetPiece(HexDirection.Center);
                if (centerPiece)
                {
                    tileData.Pieces[(int)HexDirection.Center].LookDirection = centerPiece.LookDirection;
                }

                tiles.Add(tileData);
            }
            mapData.Tiles = tiles.ToArray();
            mapData.Gimmicks = HexTileMap.Gimmicks.Values.ToArray();

            foreach (var weather in HexTileMap.Weathers)
            {
                mapData.Weathers.Add(new WeatherData()
                {
                    ActionPoint = weather.Key,
                    Weather = weather.Value
                });
            }

            OnMapSaved?.Invoke();

            return JsonUtility.ToJson(mapData);
        }

        /// <summary>
        /// HexTileMap에 json을 적용
        /// </summary>
        /// <param name="json">JSON 규격</param>
        public static void Apply(string json)
        {
            HexTileMap.Clear();

            Map mapData = JsonUtility.FromJson<Map>(json);

            // Map 정보 적용
            HexTileMap.RefillCount = mapData.RefillCount;
            HexTileMap.ActionPoint = mapData.ActionPoint;
            HexTileMap.DefaultWeather = mapData.DefaultWeather;
            HexTileMap.StartPos = mapData.StartPos;
            HexTileMap.EndPos = mapData.EndPos;

            // 타일 설치
            foreach (MapTile tile in mapData.Tiles)
            {
                HexTileMap.AddTile(tile.Type, tile.GridPos);
            }

            // 기물 설치
            // 1. 기물은 설치된 시점에 주변에 타일들과 기믹을 연동하므로, 타일이 완전히 설치된 뒤에 설치돼야 함.
            foreach (MapTile tile in mapData.Tiles)
            {
                HexTile hexTile = HexTile.GetTile(tile.GridPos);
                for (int i = 0; i < tile.Pieces.Length; i++)
                {
                    // 기물 정보 적용
                    var pieceData = tile.Pieces[i];
                    if (pieceData.Type == PieceType.None) continue;

                    Piece piece = HexTileMap.AddPiece(hexTile, (HexDirection)i, pieceData.Type, pieceData.LookDirection);
                    if (string.IsNullOrEmpty(pieceData.Data)) continue;
                    piece.SpecialData = pieceData.Data;
                }
            }
            
            // 기물 목표 위치 연결
            foreach (var grids in mapData.PieceToTargets)
            {
                HexTile.GetTile(grids.PiecePos).GetPiece(HexDirection.Center).Target = grids.TargetPos;
            }

            // 기믹 연결
            foreach (GimmickData data in mapData.Gimmicks)
            {
                HexTileMap.Gimmicks.Add(data.Target.GridPos, data);
            }
            
            // 날씨 추가
            foreach (WeatherData data in mapData.Weathers)
            {
                HexTileMap.Weathers.Add(data.ActionPoint, data.Weather);
            }

            OnMapLoaded?.Invoke();
        }

        
        #region ◇ Save 구조화용 class ◇
        [Serializable]
        private class Map
        {
            public int RefillCount = 3;
            public int ActionPoint = 5;
            public WeatherType DefaultWeather = WeatherType.Sunny;
            
            public Vector2Int StartPos = Vector2Int.zero;
            public Vector2Int EndPos = Vector2Int.zero;

            public MapTile[] Tiles = { };

            public GimmickData[] Gimmicks = { };

            public List<PieceToTarget> PieceToTargets = new();
            public List<WeatherData> Weathers = new();
        }

        [Serializable]
        private class WeatherData
        {
            public int ActionPoint = 0;
            public WeatherType Weather;
        }
        [Serializable]
        private class PieceToTarget
        {
            public Vector2Int PiecePos = Vector2Int.zero;
            public Vector2Int TargetPos = Vector2Int.zero;
        }
        [Serializable]
        private class MapTile
        {
            public TileType Type = TileType.None;
            public Vector2Int GridPos = Vector2Int.zero;

            public MapPiece[] Pieces =
            {
                new(),
                new(),
                new(),
                new(),
                new(),
                new(),
                new(),
            };
        }
        [Serializable]
        private class MapPiece
        {
            public PieceType Type = PieceType.None;
            public HexDirection LookDirection = HexDirection.East;
            public string Data = string.Empty;
        }
        #endregion
    }
}