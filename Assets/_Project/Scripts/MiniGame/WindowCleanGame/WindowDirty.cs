using CocoDoogy.Audio;
using CocoDoogy.MiniGame.WindowCleanGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.MiniGame
{
    public class WindowDirty : CanMoveImage
    {
        private WindowCleanMiniGame parent = null;


        void Start()
        {
            SetOntlineVisible();
        }

        void SetOntlineVisible()
        {
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(10, 10);
        }


        public void Init(WindowCleanMiniGame parent, Sprite sprite)
        {
            this.parent = parent;
            image.sprite = sprite;
        }


        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SetOntlineVisible();
            List<RaycastResult> raylist = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raylist);
            bool iswindowSlot = false;
            foreach (RaycastResult result in raylist)
            {
                if (result.gameObject == gameObject) continue;
                if (result.gameObject.GetComponent<WindowSlot>())
                {
                    iswindowSlot = true;
                    break;
                }
            }
            //놓는 자리에 WindowSlot없으면 사라짐
            if(!iswindowSlot)
            {
                SfxManager.PlaySfx(SfxType.Minigame_DropTrash);
                    parent.DestroyDirty(this);
            }
        }
    }
}
