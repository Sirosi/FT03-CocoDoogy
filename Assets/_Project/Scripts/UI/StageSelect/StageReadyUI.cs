using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.StageSelect.Item;
using CocoDoogy.UI.Popup;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageReadyUI : MonoBehaviour
    {
        private int selectedStage;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI title;
        
        [Header("Stage Information")]
        [SerializeField] private RectTransform page1;
        [SerializeField] private RectTransform page2;
        private bool isFirstPage;
        
        [Header("StageInfo Helps")]
        [SerializeField] private RectTransform content;
        [SerializeField] private StageData[] stageInfos;
        
        [Header("Ranks")]
        [SerializeField] private GameObject[] ranks;
        private TextMeshProUGUI[] rankTexts;
        private CommonButton[] replayButtons;
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;

        private void Awake()
        {
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnEnable()
        {
            selectedStage = StageSelectManager.SelectedStage;
            title.text = $"Stage{selectedStage}";
            
            isFirstPage = true;
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);
            
            StageInfoHelps();
        }
        
        private async void OnPageChangeButtonClicked()
        {
            if (isFirstPage)
            {
                await WindowAnimation.TurnPage(page1);
                page2.gameObject.SetActive(true);
                isFirstPage = false;
            }
            else if (!isFirstPage)
            {
                await WindowAnimation.TurnPage(page2);
                page1.gameObject.SetActive(true);
                isFirstPage = true;
            }
        }
        
        private void StageInfoHelps()
        {
            StageData stageInfo = stageInfos[selectedStage - 1];
            for (int i = content.childCount - 1; i >= 0; --i)
            {
                Destroy(content.GetChild(i).gameObject);
            }
            // foreach (var prefab in stageInfo.contentPrefabs)
            // {
            //     Instantiate(prefab, content);
            // }
        }
        
        private async void OnStartButtonClicked()
        {
            bool isReady = await OnConsumeTicketAsync();
            if (isReady)
            {
                Loading.LoadScene($"InGame");
            }
            else
            {
                // TODO : 티켓이 부족하면 메세지를 띄우게만 해뒀는데 여기에서 상점으로 연결까지 할 수도?
                MessageDialog.ShowMessage(
                    "티켓 부족", 
                    "티켓이 부족하여 게임을 진행할 수 없습니다.",
                    DialogMode.Confirm,
                    null);
            }
        }
        
        private async Task<bool> OnConsumeTicketAsync()
        {
            return await FirebaseManager.Instance.UseTicketAsync();
        }
    }
}
