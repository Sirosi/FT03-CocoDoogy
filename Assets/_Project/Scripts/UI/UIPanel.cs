using CocoDoogy.Data;
using UnityEngine;

namespace CocoDoogy.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        public abstract void OpenPanel();
        protected abstract void ClosePanel();
    }
}