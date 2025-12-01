using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    public interface IInteractable
    {
        public bool CanInteract { get; }

        public Sprite Icon { get; }

        public void OnInteractClicked();
    }
}