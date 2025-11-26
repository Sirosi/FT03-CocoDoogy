using CocoDoogy.Audio;
using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class TeleportCommand : CommandBase
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


        public TeleportCommand(object param) : base(CommandType.Teleport, param)
        {
            var poses = ((Vector2Int, Vector2Int))param;
            PrePos = poses.Item1;
            NextPos = poses.Item2;
        }


        public override void Execute()
        {
            PlayerHandler.Deploy(NextPos);
            SfxManager.PlaySfx(SfxType.Weather_Wind);
        }

        public override void Undo()
        {
            PlayerHandler.Deploy(PrePos);
        }
    }
}