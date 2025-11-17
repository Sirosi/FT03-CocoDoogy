using CocoDoogy.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect
{
    public class StageLists : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private CommonButton prevButton;
        [SerializeField] private CommonButton nextButton;

        [Header("Stage Pages")]
        [SerializeField] private int maxPage;
        [SerializeField] private RectTransform viewPort;
        [SerializeField] private Vector3 pageStep;
        private int currentPage;
        private Vector3 targetPos;




        private void Awake()
        {
            prevButton.onClick.AddListener(PrevPage);
            nextButton.onClick.AddListener(NextPage);
            
            currentPage = 1;
            targetPos = viewPort.localPosition;
        }

        private void OnEnable()
        {
            MovePage();
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
            viewPort.DOLocalMove(targetPos, 0.25f).SetEase(Ease.OutCubic);

            if (currentPage == maxPage)
            {
                nextButton.SetInteractable(false);
            }
            else
            {
                nextButton.SetInteractable(true);
            }

            if (currentPage <= 1)
            {
                prevButton.SetInteractable(false);
            }
            else
            {
                prevButton.SetInteractable(true);
            }
        }
        
    }
}
