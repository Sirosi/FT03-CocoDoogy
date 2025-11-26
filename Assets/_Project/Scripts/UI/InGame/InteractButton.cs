using CocoDoogy.GameFlow.InGame;
using System;
using UnityEngine;

namespace CocoDoogy.UI.InGame
{
    /// <summary>
    /// InGame 내에서 래버, 버튼 등의 트리거를 동작시키기 위한 상호작용 버튼 핸들러
    /// </summary>
    public class InteractButton: MonoBehaviour
    {
        [SerializeField] private CommonButton button;


        private Action action = null;


        void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
            button.gameObject.SetActive(false);

            InGameManager.OnInteractChanged += OnInteractChanged;
        }
        void OnDestroy()
        {
            InGameManager.OnInteractChanged -= OnInteractChanged;
        }


        private void OnButtonClicked()
        {
            action?.Invoke();
        }


        private void OnInteractChanged(Action callback)
        {
            button.gameObject.SetActive((action = callback) != null);
        }
    }
}
