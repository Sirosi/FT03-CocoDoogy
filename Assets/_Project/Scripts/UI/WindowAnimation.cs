using CocoDoogy.Audio;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

namespace CocoDoogy.UI
{
    public static class WindowAnimation
    {
        public static void SwipeWindow(RectTransform rt)
        {
            //SfxManager.Instance.PlaySfx(SfxType.Sfx1); // TODO: 다시 넣어야 함
            
            rt.DOKill();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rt.DOAnchorPos(new Vector2(0, Screen.height), 0.5f).SetEase(Ease.OutCubic));
            sequence.AppendCallback(() => rt.gameObject.SetActive(false));
        }

        public static void CloseWindow(Transform rt)
        {
            rt.DOKill();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rt.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutCubic));
            sequence.AppendCallback(() => rt.gameObject.SetActive(false));
        }

        public static async Task TurnPage(RectTransform rt)
        {
            rt.DOKill();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rt.DOScale(new Vector2(0, 1), 0.5f).SetEase(Ease.OutCubic));
            sequence.AppendCallback(() => rt.gameObject.SetActive(false));
            
            await sequence.AsyncWaitForCompletion();
        }
    }
}
