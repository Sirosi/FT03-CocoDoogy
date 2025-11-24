namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class SandCountCommand: CommandBase
    {
        public override bool IsUserCommand => false;
        
        
        /// <summary>
        /// 이전 모래 카운터
        /// </summary>
        public int PreCount = 0;
        /// <summary>
        /// 이후 모래 카운터
        /// </summary>
        public int NextCount = 0;


        public SandCountCommand(object param): base(CommandType.SandCount, param)
        {
            var data = ((int, int))param;
            PreCount = data.Item1;
            NextCount = data.Item2;
        }


        public override void Execute()
        {
            PlayerHandler.SandCount = NextCount;
        }

        public override void Undo()
        {
            PlayerHandler.SandCount = PreCount;
        }
    }
}