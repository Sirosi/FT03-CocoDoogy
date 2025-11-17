namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class RegenCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        public int Regen = 0;
        
        
        public RegenCommand(object param) : base(CommandType.Regen, param)
        {
            Regen = (int)param;
        }

        
        public override void Execute()
        {
            InGameManager.RegenActionPoint(Regen, false);
        }

        public override void Undo()
        {
            InGameManager.ConsumeActionPoint(Regen, false);
        }
    }
}