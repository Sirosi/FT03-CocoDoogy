using CocoDoogy.Audio;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    public class IncreaseCommand: CommandBase
    {
        public override bool IsUserCommand => false;


        public int Regen = 0;
        
        
        public IncreaseCommand(object param) : base(CommandType.Increase, param)
        {
            Regen = (int)param;
        }

        
        public override void Execute()
        {
            HexTileMap.ActionPoint += Regen;
            InGameManager.RegenActionPoint(Regen, false);
        }

        public override void Undo()
        {
            HexTileMap.ActionPoint -= Regen;
            InGameManager.ConsumeActionPoint(Regen, false);
        }
    }
}