using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class SlideCommand: CommandBase
    {
        public override bool IsUserCommand => false;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection MoveDirection = HexDirection.East;


        public SlideCommand(object param): base(CommandType.Slide, param)
        {
            MoveDirection = (HexDirection)param;
        }


        public override void Execute()
        {
            PlayerHandler.LookDirection = MoveDirection;
            Vector2Int nextPos = PlayerHandler.GridPos.GetDirectionPos(MoveDirection);
            PlayerHandler.Slide(nextPos);
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = MoveDirection;
            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(MoveDirection.GetMirror());
            PlayerHandler.Slide(prePos);
        }
    }
}