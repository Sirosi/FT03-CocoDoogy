using CocoDoogy.Data;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.StageSelect.Page;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class StageInfoPanel : MonoBehaviour
    {
        private int selectedStage;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI title;
        
        [Header("Stage Information")]
        [SerializeField] private StageInfoPage commonInfoPage;
        [SerializeField] private StageInfoPage detailInfoPage;
        
        /*-[Header("Ranks")]
        [SerializeField] private GameObject[] ranks;
        private TextMeshProUGUI[] rankTexts;
        private CommonButton[] replayButtons;*/
        
        [Header("Buttons")]
        [SerializeField] private CommonButton pageChangeButton;
        [SerializeField] private CommonButton startButton;
        
        [SerializeField] private CommonButton replayButton; // TODO: 테스트용 버튼


        private StageData stageData = null;


        protected void Awake()
        {
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }


        public void Show(StageData data)
        {
            if (!data) return;
            
            gameObject.SetActive(true);

            title.text = (stageData = data).stageName;
            
            commonInfoPage.Show(stageData);
            detailInfoPage.Close();
        }
        
        
        private void OnPageChangeButtonClicked()
        {
            if (commonInfoPage.gameObject.activeSelf)
            {
                commonInfoPage.Close(() => detailInfoPage.Show(stageData));
            }
            else
            {
                detailInfoPage.Close(() => commonInfoPage.Show(stageData));
            }
        }
        
        
        private async void OnStartButtonClicked()
        {
            bool isReady = await OnConsumeTicketAsync();
            if (isReady)
            {
                Loading.LoadScene("InGame");
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
            // TODO: 나중에 UseTicketAsync를 static 형태로 변경해야 함
            try
            {
                return await FirebaseManager.Instance.UseTicketAsync();
            }
            catch
            {
                return true;
            }
        }
    }
}
