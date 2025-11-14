using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    /// <summary>
    /// Button용 트리거<br/>
    /// 동작 이후 몇 행동력 뒤에 장비를 정지함
    /// </summary>
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
            IsOn = true;
            // TODO: 버튼 누르는 소리가 들려야 함
            // TODO: 타이머 돌려야 함
        }
        public override void UnInteract()
        {
            buttonObject.localPosition = Vector3.zero;
            IsOn = false;
        }
    }
}