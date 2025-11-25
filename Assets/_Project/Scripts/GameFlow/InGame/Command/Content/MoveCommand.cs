using CocoDoogy.Tile;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class MoveCommand: CommandBase
    {
        public override bool IsUserCommand => true;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection Dir = HexDirection.East;


        public MoveCommand(object param): base(CommandType.Move, param)
        {
            Dir = (HexDirection)param;
        }


        public override void Execute()
        {
            InGameManager.ConsumeActionPoint(HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost);
            
            PlayerHandler.LookDirection = Dir;
            Vector2Int nextPos = PlayerHandler.GridPos.GetDirectionPos(Dir);
            PlayerHandler.Move(nextPos);
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = Dir;
            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(Dir.GetMirror());
            PlayerHandler.Move(prePos);
            
            InGameManager.RegenActionPoint(HexTile.GetTile(PlayerHandler.GridPos).CurrentData.RealMoveCost);
        }
    }
}