using CocoDoogy.Data;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class UndoItemCommand : CommandBase
    {
        // TODO : Execute, Undo 기능이 아이템에도 적용되기 위해서 여기서 아이템 사용이 되어야함.
        public override bool IsUserCommand => true;
        private ItemData Data { get; }
        public UndoItemCommand(object param) : base(CommandType.Undo, param)
        {
            Data = (ItemData)param;
        }

        public override void Execute()
        {
            CommandManager.UndoCommandAuto();
            ItemHandler.SetValue(Data, false);
            PlayerHandler.IsBehaviour = true;
        }

        public override void Undo()
        {
            CommandManager.RedoCommandAuto();
            ItemHandler.SetValue(Data, true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}