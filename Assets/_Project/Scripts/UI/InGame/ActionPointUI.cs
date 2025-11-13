using CocoDoogy.GameFlow.InGame;
using UnityEngine;
using TMPro;

namespace CocoDoogy.UI.InGame
{
    public class ActionPointUI: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;


        void OnEnable()
        {
            InGameManager.OnActionPointChanged += OnActionPointChanged;
        }
        void OnDisable()
        {
            InGameManager.OnActionPointChanged -= OnActionPointChanged;
        }


        private void OnActionPointChanged(int point)
        {
            text.SetText($"{point} / {5}");
        }
    }
}