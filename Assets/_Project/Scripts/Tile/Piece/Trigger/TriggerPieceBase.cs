using CocoDoogy.LifeCycle;
using CocoDoogy.Tile.Gimmick;
using UnityEngine;

namespace CocoDoogy.Tile.Piece.Trigger
{
    public abstract class TriggerPieceBase: MonoBehaviour, ISpawn<Piece>, IRelease<Piece>
    {
        protected HexTile Parent { get; private set; }
        protected Piece Piece { get; private set; }

        public virtual bool IsOn { get; set; }
        
        
        public void OnSpawn(Piece piece)
        {
            Parent = (Piece = piece).Parent;
        }

        public abstract void OnRelease(Piece data);


        public abstract void Interact();
        public abstract void UnInteract();


        protected void ChangeTrigger(bool isOn)
        {
            GimmickExecutor.ExecuteFromTrigger(Parent.GridPos, isOn);
        }
    }
}