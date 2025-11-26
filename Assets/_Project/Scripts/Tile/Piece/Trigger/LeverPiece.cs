using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// LeverType용 트리거<br/>
    /// 토글 형태로 동작
    /// </summary>
    public class LeverPiece : TriggerPieceBase
    {
        [SerializeField] private Transform lever;


        public override bool IsOn
        {
            get => isOn;
            set
            {
                lever.localRotation = Quaternion.Euler(0, (isOn = value) ? 180 : 0, 0);
            }
        }


        private bool isOn = false;


        public override void OnRelease(Piece data)
        {
            IsOn = false;
        }
        

        public override void Interact()
        {
            IsOn = !IsOn;
            // TODO: 토글 소리가 들려야 함
        }
        public override void UnInteract() => Interact();
    }
}