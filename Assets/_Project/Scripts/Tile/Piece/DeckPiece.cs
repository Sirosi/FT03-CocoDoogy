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
        public bool PreDocked => preDocked;


        private bool isDocked = false;
        private bool preDocked = false;
        
        
        public void OnDataInsert(string data)
        {
            if (!bool.TryParse(data, out bool docked)) docked = false;
            boatObject.SetActive(IsDocked = preDocked = docked);
        }

        public void OnExecute()
        {
            boatObject.SetActive(IsDocked = !IsDocked);
        }
    }
}