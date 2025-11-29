using CocoDoogy.Data;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class UndoItemCommand : CommandBase
    {
        public override bool IsUserCommand => true;
        private ItemEffect Effect
        {
            get => itemEffect;
            set => itemEffect = value;
        }
        
        [SerializeField] private ItemEffect itemEffect;
        
        public UndoItemCommand(object param) : base(CommandType.Undo, param)
        {
            Effect = (ItemEffect)param;
        }

        public override void Execute()
        {
            CommandManager.UndoCommandAuto();
            
            Debug.Log(DataManager.GetReplayItem(Effect));
            
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), false);
            PlayerHandler.IsBehaviour = true;
        }

        public override void Undo()
        {
            CommandManager.RedoCommandAuto();
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}