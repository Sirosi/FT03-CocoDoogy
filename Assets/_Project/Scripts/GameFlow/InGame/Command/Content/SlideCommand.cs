using CocoDoogy.Tile;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class SlideCommand: CommandBase
    {
        public override bool IsUserCommand => false;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection Dir = HexDirection.East;


        public SlideCommand(object param): base(CommandType.Slide, param)
        {
            Dir = (HexDirection)param;
        }


        public override void Execute()
        {
            PlayerHandler.LookDirection = Dir;
            Vector2Int nextPos = PlayerHandler.GridPos.GetDirectionPos(Dir);
            PlayerHandler.Slide(nextPos);
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = Dir;
            Vector2Int prePos = PlayerHandler.GridPos.GetDirectionPos(Dir.GetMirror());
            PlayerHandler.Slide(prePos);
        }
    }
}