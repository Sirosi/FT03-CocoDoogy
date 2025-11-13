using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Outline))]
    public class CanMoveImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        protected Animator anim;
        protected Image image;
        protected Outline outline;

        protected Vector2 orginalOutliner = new Vector2(0,0);
        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            image = GetComponent<Image>();
            outline = GetComponent<Outline>();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            outline.effectDistance = new Vector2(10f, 10f);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            outline.effectDistance = orginalOutliner;
        }

        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {

        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            //이미지가 따라오도록
            transform.position = eventData.position;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            //이미지가 놓은 자리에 위치하도록&&쓰레기통에 닿으면 삭제
            transform.position = eventData.position;
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            
            
        }

    }
}
