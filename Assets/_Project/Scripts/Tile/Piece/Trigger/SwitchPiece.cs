using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    public class SwitchPiece : TriggerPieceBase
    {
        [SerializeField] private Transform lever;


        public override bool IsOn
        {
            get => isOn;
            set
            {
                lever.rotation = Quaternion.Euler(0, 0, (isOn = value) ? 60 : 0);
            }
        }


        private bool isOn = false;


        public override void OnRelease(Piece data)
        {
            IsOn = false;
        }
        

        public override void Interact()
        {
            ChangeTrigger(IsOn = !IsOn);
        }
        public override void UnInteract() => Interact();
    }
}