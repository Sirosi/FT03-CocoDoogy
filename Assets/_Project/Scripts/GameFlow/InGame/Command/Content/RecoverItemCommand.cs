using CocoDoogy.Data;
using CocoDoogy.EmoteBillboard;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class RecoverItemCommand : CommandBase
    {
        // TODO : Execute, Undo 기능이 아이템에도 적용되기 위해서 여기서 아이템 사용이 되어야함.
        public override bool IsUserCommand => true;

        private ItemEffect Effect
        {
            get => itemEffect;
            set => itemEffect = value;
        }

        [SerializeField] private ItemEffect itemEffect;

        private const int Recover = 1;


        public RecoverItemCommand(object param) : base(CommandType.Recover, param)
        {
            Effect = (ItemEffect)param;
        }

        public override void Execute()
        {
            InGameManager.RegenActionPoint(Recover, false);

            Debug.Log(DataManager.GetReplayItem(Effect));

            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), false);
            PlayerHandler.IsBehaviour = true;

            // 만족 감정 트리거 (비상식량 아이템 사용)
            EmotionSystemHandler.TriggerActionPointRecovered();
        }

        public override void Undo()
        {
            InGameManager.ConsumeActionPoint(Recover, false);
            ItemHandler.SetValue(DataManager.GetReplayItem(Effect), true);
            PlayerHandler.IsBehaviour = false;
        }
    }
}