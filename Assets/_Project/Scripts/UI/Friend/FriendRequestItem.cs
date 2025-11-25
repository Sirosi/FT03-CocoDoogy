using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Friend
{
    public class FriendRequestItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button presentButton;
        [SerializeField] private Button deleteButton;
        private Action<string> onAccept;
        private Action<string> onReject;
        private Action<string> onCancel;
        private Action<string> onDelete;
        private Action<string> onPresent;
        private string uid;

        /// <summary>
        /// Received Request에 사용하는 초기화 메서드
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="receivedUid"></param>
        /// <param name="acceptCallback"></param>
        /// <param name="rejectCallback"></param>
        public void ReceivedInit(string nickname, string receivedUid, Action<string> acceptCallback, Action<string> rejectCallback)
        {
            uid = receivedUid;
            nicknameText.text = nickname;
            onAccept = acceptCallback;
            onReject = rejectCallback;

            acceptButton.onClick.AddListener(() => onAccept?.Invoke(uid));
            rejectButton.onClick.AddListener(() => onReject?.Invoke(uid));
        }

        /// <summary>
        /// Sent Request에 사용하는 초기화 메서드
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="sentUid"></param>
        /// <param name="cancelCallback"></param>
        public void SentInit(string nickname, string sentUid, Action<string> cancelCallback)
        {
            uid = sentUid;
            nicknameText.text = nickname;
            onCancel = cancelCallback;

            cancelButton.onClick.AddListener(() => onCancel?.Invoke(sentUid));
        }

        public void FriendInit(string nickname, string deletedUid, Action<string> presentCallback, Action<string> cancelCallback)
        {
            uid = deletedUid;
            nicknameText.text = nickname;
            onPresent = presentCallback;
            onDelete = cancelCallback;

            presentButton.onClick.AddListener(() => onPresent?.Invoke(uid));
            deleteButton.onClick.AddListener(() => onDelete?.Invoke(uid));
        }
    }
}