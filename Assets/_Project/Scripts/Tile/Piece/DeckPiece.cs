using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    [RequireComponent(typeof(Piece))]
    public class DeckPiece: MonoBehaviour, ISpecialPiece
    {
        [SerializeField] private GameObject boatObject;


        public bool IsDocked
        {
            get => isDocked;
            set => boatObject.SetActive(isDocked = value);
        }


        private bool isDocked = false;
        
        
        public void OnDataInsert(string data)
        {
            if (!bool.TryParse(data, out bool docked)) docked = false;
            boatObject.SetActive(IsDocked = docked);
        }

        public void OnExecute()
        {
            boatObject.SetActive(IsDocked = !IsDocked);
        }
    }
}