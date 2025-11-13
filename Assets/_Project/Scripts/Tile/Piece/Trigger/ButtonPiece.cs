using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    public class ButtonPiece : TriggerPieceBase
    {
        [SerializeField] private Transform buttonObject;


        public override void OnRelease(Piece data)
        {
            buttonObject.localPosition = Vector3.zero;
        }
        

        public override void Interact()
        {
            buttonObject.localPosition = Vector3.down * 0.1f;
            // TODO: 타이머 돌려야 함
            ChangeTrigger(IsOn = true);
        }
        public override void UnInteract()
        {
            buttonObject.localPosition = Vector3.zero;

            ChangeTrigger(IsOn = false);
        }
    }
}