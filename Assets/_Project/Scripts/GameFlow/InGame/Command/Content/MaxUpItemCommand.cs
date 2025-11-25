using CocoDoogy.Data;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    // TODO : Execute, Undo 기능이 아이템에도 적용되기 위해서 여기서 아이템 사용이 되어야함.
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