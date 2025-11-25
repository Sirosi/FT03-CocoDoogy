namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class RecoverCommand : CommandBase
    {
        public override bool IsUserCommand => false;

        public int Recover = 0;
        
        public RecoverCommand(object param) : base(CommandType.Recover, param)
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