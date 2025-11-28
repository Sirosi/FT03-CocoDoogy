using CocoDoogy.Data;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class UndoItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;


        private ItemData Data { get; } = null;
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