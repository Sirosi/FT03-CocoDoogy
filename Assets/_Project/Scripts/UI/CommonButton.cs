using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    public class CommonButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public UnityEvent onClick;
        
        
        private RectTransform rect;
        private Image image;
        private Color buttonColor;

        public bool interactable = true;
        
        private bool isHovered = false;

        
        void Awake()
        {
            rect = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            buttonColor = image.color;
        }


        public void SetInteractable(bool value)
        {
            interactable = value;

            if (!interactable)
            {
                image.DOColor(new Color(1, 1, 1, 0.5f), 0.2f).SetEase(Ease.OutCubic).SetId(this);
                rect.localScale = Vector3.one;
            }
            else
            {
                image.DOColor(buttonColor, 0.2f).SetEase(Ease.OutBack).SetId(this);
            }
        }
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable) return;
            
            isHovered = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable) return;
            
            isHovered = false;
        }
        public void OnPointerDown(PointerEventData data)
        {
            if (!interactable) return;
            
            DOTween.Kill(this);
            
            rect.DOScale(0.95f, 0.15f).SetEase(Ease.OutCubic).SetId(this);
            image.DOColor(buttonColor * 0.8f, 0.15f).SetEase(Ease.OutCubic).SetId(this);
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (!interactable) return;
            
            DOTween.Kill(this);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOScale(new Vector2(1.1f, 0.9f), 0.1f).SetEase(Ease.OutCubic));
            sequence.Append(rect.DOScale(new Vector2(0.9f, 1.1f), 0.1f).SetEase(Ease.OutCubic));
            sequence.Append(rect.DOScale(Vector2.one, 0.1f).SetEase(Ease.OutCubic));
            sequence.SetId(this);
            sequence.Play();

            image.DOColor(buttonColor, 0.2f).SetEase(Ease.OutBack).SetId(this);
            
            if (!isHovered) return;
            onClick?.Invoke();
        }
    }
}