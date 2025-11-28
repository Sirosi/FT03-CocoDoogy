using CocoDoogy.Audio;
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
        /// 이전 위치
        /// </summary>
        public Vector2Int PrePos = Vector2Int.zero;
        /// <summary>
        /// 다음 위치
        /// </summary>
        public Vector2Int NextPos = Vector2Int.zero;


        public SlideCommand(object param): base(CommandType.Slide, param)
        {
            var data = ((Vector2Int, Vector2Int))param;
            PrePos = data.Item1;
            NextPos = data.Item2;
        }


        public override void Execute()
        {
            SfxManager.PlaySfx(SfxType.Interaction_Sliding);
            PlayerHandler.Slide(NextPos);
        }

        public override void Undo()
        {
            PlayerHandler.Slide(PrePos);
        }
    }
}