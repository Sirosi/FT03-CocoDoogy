using CocoDoogy.Data;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class RecoverItemCommand : CommandBase
    {
        // TODO : Execute, Undo 기능이 아이템에도 적용되기 위해서 여기서 아이템 사용이 되어야함.
        public override bool IsUserCommand => true;
        private ItemData Data { get; }
        
        private const int Recover = 1;
        
        
        public RecoverItemCommand(object param) : base(CommandType.Recover, param)
        {
            Data = (ItemData)param;
        }

        public override void Execute()
        {
            InGameManager.RegenActionPoint(Recover, false);
            ItemHandler.SetValue(Data, false);
            PlayerHandler.IsBehaviour = true;
        }

        public override void Undo()
        {
            InGameManager.ConsumeActionPoint(Recover, false);
            ItemHandler.SetValue(Data, true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}