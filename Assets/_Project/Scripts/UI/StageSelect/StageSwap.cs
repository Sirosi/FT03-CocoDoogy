using CocoDoogy.Core;
using CocoDoogy.Data;
using DG.Tweening;
using Lean.Pool;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.StageSelect
{
    public class StageSwap : MonoBehaviour, IEndDragHandler
    {
        [Header("Stage Buttons")]
        [SerializeField] private StageListPage stageListPage;
        
        [Header("Pages Movement")]
        [SerializeField] private RectTransform contents;


        private int currentPage = 1;
        private int maxPage = 1;
        
        private Vector3 targetPos;
        private Vector3 pageStep;
        private float dragThreshould;

        private Theme nowTheme = Theme.None;





        public void Show(Theme theme)
        {
            gameObject.SetActive(true);
            nowTheme = theme;
            maxPage = Mathf.FloorToInt(DataManager.GetStageData(nowTheme).Count / (float)StageListPage.LIST_SIZE);
            
            currentPage = 0;
            targetPos = contents.anchoredPosition;
            float pageWidth = ((RectTransform)contents.GetChild(0)).rect.width;
            pageStep = new Vector3(pageWidth, 0, 0);
            dragThreshould = Screen.width / 15;
            
            MovePage(1);
        }

        

        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshould)
            {
                if (eventData.position.x > eventData.pressPosition.x) PrevPage();
                else NextPage();
            }
            else MovePage(currentPage);
        }

        
        
        private void PrevPage()
        {
            if (currentPage <= 0) return;
            
            targetPos += pageStep;
            MovePage(currentPage - 1);
        }
        
        private void NextPage()
        {
            if (currentPage >= maxPage) return;
            
            targetPos -= pageStep;
            MovePage(currentPage + 1);
        }

        private void MovePage(int page)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(contents.DOMove(targetPos, 0.2f));
            contents.DOLocalMove(targetPos, 0.75f).SetEase(Ease.OutCubic);
        }
    }
}
