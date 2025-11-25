using CocoDoogy.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoDoogy
{
    public class TESTButtonAudio : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            print("버튼 누름");
            SfxManager.PlaySfx(SfxType.UI_ButtonDown);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            print("버튼 뗌");
            SfxManager.PlaySfx(SfxType.UI_ButtonUp1);
        }
    }
}
