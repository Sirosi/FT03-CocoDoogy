using CocoDoogy.Tile.Gimmick.Data;
using UnityEngine;

namespace CocoDoogy.Tile.Gimmick
{
    public static class GimmickExecutor
    {
        public static void ExecuteFromTrigger(Vector2Int gridPos, bool isOn)
        {
            GimmickData[] gimmicks = HexTileMap.GetTriggers(gridPos);
            if (gimmicks.Length <= 0) return;

            foreach (GimmickData gimmick in gimmicks)
            {
                gimmick.GetTrigger(gridPos).IsTriggered = isOn;
                    
                bool gimmickOn = true;
                foreach (var trigger in gimmick.Triggers)
                {
                    if (trigger.IsTriggered == trigger.IsReversed)
                    {
                        gimmickOn = false;
                        break;
                    }
                }

                if (gimmick.IsTriggered == gimmickOn) continue; // 같으면 동작해선 안 됨
                gimmick.IsTriggered = gimmickOn;
                HexTile terget = HexTile.GetTile(gimmick.Target.GridPos);

                switch(gimmick.Type)
                {
                    case GimmickType.TileRotate:
                        HexRotate rotate = gimmick.Effect.Rotate;
                        if (!gimmick.IsTriggered)
                        {
                            rotate = rotate.GetMirror();
                        }
                        terget.Rotate(rotate);
                        break;
                    case GimmickType.PieceChange:
                        // TODO: 기존 기물을 저장해야해서 좀 걸릴 듯 함
                        break;
                    case GimmickType.PieceDestroy:
                        break;
                }
            }
        }
    }
}