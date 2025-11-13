using CocoDoogy.MiniGame.TrashGame;
using CocoDoogy.UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CocoDoogy.Audio;

namespace CocoDoogy.MiniGame
{
    public class Trash : CanMoveImage
    {
        private TrashMiniGame parent = null;


        public void Init(TrashMiniGame trashMiniGame, Sprite sprite)
        {
            parent = trashMiniGame;
            image.sprite = sprite;
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            SfxManager.PlaySfx(SfxType.Minigame_PickTrash);
        }

       
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            TrashCan trashCan = hitObject != null ? hitObject.GetComponent<TrashCan>() : null;
            if (trashCan != null)
            {
                SfxManager.PlaySfx(SfxType.Minigame_DropTrash);
                trashCan.ShakingWithTilt();
                parent.DestroyTrash(this);
            }
            else
            {
                SfxManager.PlaySfx(SfxType.Minigame_DropTrash);
            }
        }
    }
}
