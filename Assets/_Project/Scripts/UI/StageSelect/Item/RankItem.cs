using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.StageSelect.Item
{
    public class RankItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI resetCountText;
        [SerializeField] private TextMeshProUGUI remainAPText;
        [SerializeField] private TextMeshProUGUI clearTimeText;

        [SerializeField] private Button replayButton;
        
        /// <summary>
        /// 스테이지 선택창에서 스테이지를 선택하면 해당 스테이지의 랭킹을 띄우는 아이템의 초기화
        /// </summary>
        public void Init(string rank, string nickname, string resetCount, string remainAP, string clearTime, string replayId)
        {
            rankText.text = rank;
            nicknameText.text = nickname;
            resetCountText.text = resetCount;
            remainAPText.text = remainAP;
            clearTimeText.text = clearTime;
            
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnClickReplayStart);
            
            // TODO: 본인이 클리어한 스테이지가 별이3개 
        }
        
        /// <summary>
        /// 버튼을 눌렀을 때 리플레이를 시작하는 메서드
        /// </summary>
        private void OnClickReplayStart()
        {
            // TODO: 리플레이 씬으로 이동하여 리플레이 시작
        }
    }
}