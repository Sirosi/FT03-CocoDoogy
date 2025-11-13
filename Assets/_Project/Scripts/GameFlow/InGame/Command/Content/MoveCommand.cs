using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class MoveCommand: CommandBase
    {
        public override bool IsUserCommand => true;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection MoveDirection = HexDirection.East;


        public MoveCommand(object param): base(CommandType.Move, param)
        {
            MoveDirection = (HexDirection)param;
        }


        public override void Execute()
        {
            PlayerHandler.LookDirection = MoveDirection;
            Vector2Int nextPos = PlayerHandler.GridPos.GetDirectionPos(MoveDirection);
            PlayerHandler.Move(nextPos);
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = MoveDirection;
            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(MoveDirection.GetMirror());
            PlayerHandler.Move(prePos);
        }
    }
}