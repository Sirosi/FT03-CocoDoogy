namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class RecoverItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;

        public int Recover = 0;
        
        public RecoverItemCommand(object param) : base(CommandType.Recover, param)
        {
            Recover = (int)param;
        }

        public override void Execute()
        {
            InGameManager.RegenActionPoint(Recover, false);
        }

        public override void Undo()
        {
            InGameManager.ConsumeActionPoint(Recover, false);
        }
    }
}