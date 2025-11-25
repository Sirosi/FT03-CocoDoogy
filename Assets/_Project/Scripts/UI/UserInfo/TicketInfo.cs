using UnityEngine;
using UnityEngine.EventSystems;

namespace CocoDoogy.UI.UserInfo
{
    public class TicketInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject realTimeTicket;
        
        private void Awake()
        {
            realTimeTicket.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            realTimeTicket.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            realTimeTicket.SetActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            realTimeTicket.SetActive(false);
        }
    }
}
