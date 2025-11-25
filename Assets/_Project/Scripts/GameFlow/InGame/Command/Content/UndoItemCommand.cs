namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class UndoItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;
        public UndoItemCommand(object param) : base(CommandType.Undo, param)
        {
            
        }

        public override void Execute()
        {
            CommandManager.UndoCommandAuto();
        }

        public override void Undo()
        {
            CommandManager.RedoCommandAuto();
        }
    }
}