using CocoDoogy.Audio;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame;
using CocoDoogy.Network;
using CocoDoogy.UI.Popup;
using CocoDoogy.UI.StageSelect.Page;
using System;
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


        public bool IsOpened { get; private set; } = false;


        private StageData stageData = null;


        void Awake()
        {
            pageChangeButton.onClick.AddListener(OnPageChangeButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        void OnDisable()
        {
            IsOpened = false;
        }


        public void Show(StageData data)
        {
            if (!data) return;

            IsOpened = true;
            gameObject.SetActive(true);

            title.text = (stageData = data).stageName;
            
            BgmManager.PrepareStageBgm(data.theme);
            
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
            startButton.interactable = false;
            bool isReady = await OnConsumeTicketAsync();
            if (isReady)
            {
                InGameManager.Stage = stageData;
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
                startButton.interactable = true;
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
