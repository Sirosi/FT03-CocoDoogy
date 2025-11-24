namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class UndoCommand : CommandBase
    {
        public override bool IsUserCommand => false;
        public UndoCommand(object param) : base(CommandType.Undo, param)
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