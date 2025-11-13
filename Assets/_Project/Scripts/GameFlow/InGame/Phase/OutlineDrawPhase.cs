using CocoDoogy.Tile;
using CocoDoogy.Tile.Gimmick.Data;
using CocoDoogy.Tile.Piece;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 고정적으로 들어가야하는 기믹 등을 탐색하기 위한 Outline
    /// </summary>
    public class OutlineDrawPhase: IPhase
    {
        private readonly Stack<HexTile> filledTiles = new();
        
        
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;

            // 기존에 Outline이 들어간 타일 색 제거 
            while (filledTiles.Count > 0)
            {
                filledTiles.Pop().OffOutline();
            }
            
            // 기믹이 존재하는 타일이면, 빨간색으로
            foreach (var data in HexTileMap.Instance.Gimmicks.Values)
            {
                HexTile gimmickTile = HexTile.GetTile(data.Target.GridPos);
                gimmickTile.DrawOutline(Color.red);
                filledTiles.Push(gimmickTile);
            }

            HexTile destination = HexTile.GetTile(HexTileMap.Instance.EndPos);
            destination.DrawOutline(Color.purple);
            filledTiles.Push(destination);
            
            return true;
        }
    }
}