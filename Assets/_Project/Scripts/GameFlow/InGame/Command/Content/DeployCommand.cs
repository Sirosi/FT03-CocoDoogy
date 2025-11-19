using CocoDoogy.Tile;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    /// <summary>
    /// 게임 시작 배치에 대한 Command<br/>
    /// GridPos(Vector2Int), LookDirection(HexDirection)
    /// </summary>
    [System.Serializable]
    public class DeployCommand: CommandBase
    {
        public override bool IsUserCommand => true;
        
        
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public Vector2Int GridPos = Vector2Int.zero;
        /// <summary>
        /// 움직이는 방향
        /// </summary>
        public HexDirection LookDirection = HexDirection.East;


        public DeployCommand(object param): base(CommandType.Deploy, param)
        {
            (Vector2Int, HexDirection) data = ((Vector2Int, HexDirection))param;

            GridPos = data.Item1;
            LookDirection = data.Item2;
        }


        public override void Execute()
        {
            PlayerHandler.LookDirection = LookDirection;
            PlayerHandler.Deploy(GridPos);
        }

        public override void Undo()
        {
            PlayerHandler.LookDirection = LookDirection;
            PlayerHandler.Deploy(GridPos);
        }
    }
}