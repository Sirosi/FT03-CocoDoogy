using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace CocoDoogy
{
    public class Timer : MonoBehaviour
    {
        public TextMeshProUGUI timeText;

        private float playTime;
        private readonly bool isPaused = false;
        
        private void Start()
        {
            InvokeRepeating(nameof(IncreaseTime), 0f, 1f);
        }

        private void IncreaseTime()
        {
            if (isPaused) return;

            playTime += 1f;
            int min = (int)(playTime / 60);
            int sec = (int)(playTime % 60);
            timeText.text = $"{min:D2}:{sec:D2}";
        }
    }
}
