using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class MaxUpCommand : CommandBase
    {
        public override bool IsUserCommand => false;

        public int Delta = 0;
        
        public MaxUpCommand(object param) : base(CommandType.MaxUp, param)
        {
            Delta = (int)param;
        }
        
        public override void Execute()
        {
            HexTileMap.ActionPoint += Delta;
            InGameManager.ConsumeActionPoint(Delta, false);
        }

        public override void Undo()
        {
            HexTileMap.ActionPoint -= Delta;
            InGameManager.RegenActionPoint(Delta, false);
        }
    }
}