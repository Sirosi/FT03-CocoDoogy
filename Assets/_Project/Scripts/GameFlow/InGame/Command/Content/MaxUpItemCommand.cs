using CocoDoogy.Data;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class MaxUpItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;
        private ItemData Data { get; }
        private const int Delta = 1;
        
        public MaxUpItemCommand(object param) : base(CommandType.MaxUp, param)
        {
            Data = (ItemData)param;
        }
        
        public override void Execute()
        {
            HexTileMap.ActionPoint += Delta;
            InGameManager.ConsumeActionPoint(Delta, false);
            ItemHandler.SetValue(Data, false);
            PlayerHandler.IsBehaviour = true;
        }

        public override void Undo()
        {
            HexTileMap.ActionPoint -= Delta;
            InGameManager.RegenActionPoint(Delta, false);
            ItemHandler.SetValue(Data, true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}