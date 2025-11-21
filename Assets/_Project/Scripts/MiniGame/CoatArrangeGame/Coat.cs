using CocoDoogy.Audio;
using CocoDoogy.MiniGame.CoatArrangeGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public class Coat : CanMoveImage
    {
        public int Id;

        private RectTransform rectTransform;
        private Vector2 originalAnchoredPos;
        private Transform originalParent;
        private CoatArrangeMiniGame parent;
        public void Init(CoatArrangeMiniGame coatArrangeMiniGame)
        {
            parent = coatArrangeMiniGame;
        }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        public void SetUnInteractable()
        {
            image.raycastTarget = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            SfxManager.PlaySfx(SfxType.Minigame_PickCloth);
        }

        

        public override void OnBeginDrag(PointerEventData eventData)
        {
            originalAnchoredPos = rectTransform.anchoredPosition;
            originalParent = transform.parent;
        }


        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            // 현재 포인터 아래에 있는 모든 UI 오브젝트를 탐색
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            Coat otherCoat = null;

            foreach (var result in raycastResults)
            {
                if (result.gameObject == gameObject)
                    continue; // 자기 자신은 무시
                //다른 coat가 있으면 반환하여 otherCoat에 넣음
                if(result.gameObject.TryGetComponent<Coat>(out otherCoat))
                    break; 
            }

            if (otherCoat != null)
            {
                RectTransform otherRect = otherCoat.GetComponent<RectTransform>();
                CoatSlot otherCoatSlot = otherCoat.GetComponentInParent<CoatSlot>();
                CoatSlot thisCoatSlot = GetComponentInParent<CoatSlot>();
                Transform otherParent = otherCoat.transform.parent;
                Vector2 otherAnchoredPos = otherRect.anchoredPosition;

                // 부모 및 위치 교체 (UI 좌표 유지)
                otherCoat.transform.SetParent(originalParent, false);
                otherRect.anchoredPosition = originalAnchoredPos;
                    
                transform.SetParent(otherParent, false);
                rectTransform.anchoredPosition = otherAnchoredPos;
                
                SfxManager.PlaySfx(SfxType.Minigame_DropCloth);
                parent.OnCoatSwapped(thisCoatSlot, otherCoatSlot);
            }
            else
            {
                
                SfxManager.PlaySfx(SfxType.Minigame_DropCloth);
                transform.SetParent(originalParent, false);
                rectTransform.anchoredPosition = originalAnchoredPos;
            }
        }
    }
}
