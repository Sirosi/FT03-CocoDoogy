using CocoDoogy.Core;
using CocoDoogy.Data;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.StageSelect
{
    public class StageLists : MonoBehaviour, IEndDragHandler
    {
        [Header("Stage Buttons")]
        [SerializeField] private StageSelectButton stageSelectButtonPrefab;
        [SerializeField] private Transform[] stageSelectPages;
        
        [Header("Stage Pages")]
        [SerializeField] private int maxPage;
        private int currentPage;
        
        [Header("Pages Movement")]
        [SerializeField] private RectTransform contents;
        private Vector3 targetPos;
        private Vector3 pageStep;
        float dragThreshould;




        private void Awake()
        {
            currentPage = 1;
            targetPos = contents.anchoredPosition;
            float pageWidth = ((RectTransform)contents.GetChild(0)).rect.width;
            pageStep = new Vector3(pageWidth, 0, 0);
            dragThreshould = Screen.width / 15;
        }

        private void OnEnable()
        {
            DrawStageButtons();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshould)
            {
                if (eventData.position.x > eventData.pressPosition.x) PrevPage();
                else NextPage();
            }
            else MovePage();
        }

        
        
        private void PrevPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                targetPos += pageStep;
                MovePage();
            }
        }
        
        private void NextPage()
        {
            if (currentPage < maxPage)
            {
                currentPage++;
                targetPos -= pageStep;
                MovePage();
            }
        }

        private void MovePage()
        {
            contents.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutCubic);
        }


        private void DrawStageButtons()
        {
            Theme theme = Theme.Forest;
            int start = 1;
            int size = 5;
            int max = start + size - 1;
            foreach (var data in DataManager.GetStageData(theme))
            {
                Debug.Log(data.index);
                if (start > data.index || data.index > max) continue;
                
                StageSelectButton stageSelectButton = Instantiate(stageSelectButtonPrefab, stageSelectPages[0]);
                stageSelectButton.Init(data, 3);
            }
        }
    }
}
