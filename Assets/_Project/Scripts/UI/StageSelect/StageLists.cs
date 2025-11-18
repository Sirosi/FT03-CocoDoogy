using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.StageSelect
{
    public class StageLists : MonoBehaviour, IEndDragHandler
    {
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
    }
}
