using CocoDoogy.Audio;
using CocoDoogy.Core;
using CocoDoogy.UI.Popup;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.MiniGame
{
    public enum GetBackGroundType
    {
        InDoor,
        OutDoor,
        Fixed
    }
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Tooltip("해당 미니게임 등장 가능 계절 테마")]
        [SerializeField] private Theme themeFlag;
        [SerializeField] protected Image background;


        private Action clearCallback;


        /// <summary>
        /// 해당 테마에 사용 가능한 미니게임 여부
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public bool HasWithTheme(Theme theme) => themeFlag.HasFlag(theme);

        /// <summary>
        /// 미니게임 시작
        /// </summary>
        /// <param name="callback"></param>
        public void Open(Action callback)
        {
            clearCallback = callback;
            gameObject.SetActive(true);

            OnOpenInit();
        }


        /// <summary>
        /// 미니게임 클리어 판단을 언제해야하지?
        /// </summary>
        public void CheckClear()
        {
            if (!IsClear()) return;

            SfxManager.PlaySfx(SfxType.UI_SuccessStage);
            MessageDialog.ShowMessage("미니게임 클리어", "보상을 받으시오", DialogMode.Confirm, _ => 
            {
                Disable();
                gameObject.SetActive(false);
                clearCallback?.Invoke();
            });
            
        }

        /// <summary>
        /// 미니게임 배경 세팅
        /// </summary>
        /// <param name="sprite"></param>
        protected abstract void SetBackground(Sprite sprite);


        /// <summary>
        /// 미니게임 시작 세팅
        /// </summary>
        protected abstract void OnOpenInit();


        /// <summary>
        /// 미니게임 클리어 조건
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsClear();

        /// <summary>
        /// 미니게임 초기화
        /// </summary>
        protected abstract void Disable();
    }
}
