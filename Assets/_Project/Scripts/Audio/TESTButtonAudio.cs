using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.Audio
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
